using System.Security.Claims;
using eVote360_Pro.Application.DTOs.PoliticalAlliance.Requests;
using eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Servicio que gestiona los pactos y alianzas entre partidos políticos.
    /// Utiliza IHttpContextAccessor directamente para obtener el contexto del usuario logueado.
    /// </summary>
    public class PoliticalAllianceService : IPoliticalAllianceService
    {
        private readonly IPoliticalAlliancesRepository _allianceRepository;
        private readonly IPoliticalPartiesRepository _partyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;

        public PoliticalAllianceService(
            IPoliticalAlliancesRepository allianceRepository,
            IPoliticalPartiesRepository partyRepository,
            IHttpContextAccessor httpContextAccessor,
            IDateTimeProvider dateTimeProvider,
            IUnitOfWork unitOfWork
        )
        {
            _allianceRepository = allianceRepository;
            _partyRepository = partyRepository;
            _httpContextAccessor = httpContextAccessor;
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AllianceResponse>> GetMyAlliancesAsync()
        {
            int partyId = GetCurrentPartyId();

            var options = new QueryOptions<PoliticalAlliance>
            {
                Filter = a => a.RequesterPartyId == partyId || a.ReceiverPartyId == partyId,
                Includes = new() { a => a.RequesterParty, a => a.ReceiverParty }
            };

            var alliances = await _allianceRepository.GetAllAsync(options);
            return alliances.ToResponse();
        }

        public async Task RequestAllianceAsync(CreateAllianceRequest request)
        {
            // 1. Obtener el PartyId del usuario logueado
            int requesterPartyId = GetCurrentPartyId();

            // 2. Validar que no se alíe consigo mismo
            if (requesterPartyId == request.ReceiverPartyId)
            {
                throw new DomainException("No puede crear una solicitud de alianza hacia su propio partido político.", "Alliance.SameParty");
            }

            // 3. Llamar a AllianceOrRequestExistsAsync
            if (await _allianceRepository.AllianceOrRequestExistsAsync(requesterPartyId, request.ReceiverPartyId))
            {
                throw new DomainException("Ya existe una solicitud de alianza o un pacto vigente con este partido.", "Alliance.AlreadyExists");
            }

            var receiverParty = await _partyRepository.GetByIdAsync(request.ReceiverPartyId)
                                ?? throw new DomainException("El partido político receptor no existe.", "PoliticalParty.NotFound");

            // 4. Guardar con estado Pending
            var alliance = PoliticalAlliance.Create(requesterPartyId, request.ReceiverPartyId, receiverParty.IsActive);

            await _allianceRepository.AddAsync(alliance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AcceptAllianceAsync(int allianceId)
        {
            var alliance = await _allianceRepository.GetByIdAsync(allianceId, a => a.RequesterParty, a => a.ReceiverParty)
                            ?? throw new DomainException("La solicitud de alianza no existe.", "Alliance.NotFound");

            int partyId = GetCurrentPartyId();
            if (alliance.ReceiverPartyId != partyId)
            {
                throw new DomainException("No tiene permisos para responder a esta solicitud.", "Alliance.AccessDenied");
            }

            alliance.Accept(alliance.RequesterParty.IsActive, alliance.ReceiverParty.IsActive, _dateTimeProvider.UtcNow);

            _allianceRepository.Update(alliance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RejectAllianceAsync(int allianceId)
        {
            var alliance = await _allianceRepository.GetByIdAsync(allianceId)
                            ?? throw new DomainException("La solicitud de alianza no existe.", "Alliance.NotFound");

            int partyId = GetCurrentPartyId();
            if (alliance.ReceiverPartyId != partyId)
            {
                throw new DomainException("No tiene permisos para responder a esta solicitud.", "Alliance.AccessDenied");
            }

            alliance.Reject();

            _allianceRepository.Update(alliance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAllianceAsync(int id)
        {
            var alliance = await _allianceRepository.GetByIdAsync(id)
                            ?? throw new DomainException("La alianza no existe.", "Alliance.NotFound");

            int partyId = GetCurrentPartyId();
            if (alliance.RequesterPartyId != partyId && alliance.ReceiverPartyId != partyId)
            {
                throw new DomainException("No tiene permisos para eliminar esta alianza.", "Alliance.AccessDenied");
            }

            // Bloquear si HasActiveAlliedAssignmentsAsync es true
            bool hasAssignments = await _allianceRepository.HasActiveAlliedAssignmentsAsync(alliance.RequesterPartyId, alliance.ReceiverPartyId);
            alliance.EnsureCanBeDeleted(hasAssignments);

            _allianceRepository.Delete(alliance);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PoliticalPartyResponse>> GetAvailablePartiesForRequestAsync()
        {
            int myPartyId = GetCurrentPartyId();

            // Retornar partidos activos excluyendo el propio y aquellos con los que ya hay un pacto
            var allParties = await _partyRepository.GetAllAsync();
            
            var availableParties = new List<PoliticalParty>();
            
            foreach (var party in allParties.Where(p => p.IsActive && p.Id != myPartyId))
            {
                if (!await _allianceRepository.AllianceOrRequestExistsAsync(myPartyId, party.Id))
                {
                    availableParties.Add(party);
                }
            }

            return availableParties.ToResponse();
        }

        /// <summary>
        /// Extrae el PartyId del usuario logueado desde los claims del HttpContext.
        /// </summary>
        private int GetCurrentPartyId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var partyIdClaim = user?.FindFirst("party_id")?.Value;

            if (string.IsNullOrEmpty(partyIdClaim) || !int.TryParse(partyIdClaim, out int partyId))
            {
                throw new DomainException("No se pudo identificar el partido político del usuario logueado.", "Alliance.Unauthorized");
            }

            return partyId;
        }
    }
}
