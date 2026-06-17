using eVote360_Pro.Application.DTOs.PoliticalAlliance.Requests;
using eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.Models.Emails;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de alianzas.
    /// </summary>
    public class PoliticalAllianceService : IPoliticalAllianceService
    {
        private readonly IPoliticalAlliancesRepository _allianceRepository;
        private readonly IPoliticalPartiesRepository _partyRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public PoliticalAllianceService(
            IPoliticalAlliancesRepository allianceRepository,
            IPoliticalPartiesRepository partyRepository,
            IElectionRepository electionRepository,
            ICurrentUserService currentUserService,
            IDateTimeProvider dateTimeProvider,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IConfiguration configuration
        )
        {
            _allianceRepository = allianceRepository;
            _partyRepository = partyRepository;
            _electionRepository = electionRepository;
            _currentUserService = currentUserService;
            _dateTimeProvider = dateTimeProvider;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<AllianceResponse>> GetMyAlliancesAsync()
        {
            int partyId = GetCurrentPartyId();
            var options = new QueryOptions<PoliticalAlliance>
            {
                Filter = a => a.RequesterPartyId == partyId || a.ReceiverPartyId == partyId,
                Includes = new() { a => a.RequesterParty, a => a.ReceiverParty },
                IsTracking = false,
            };
            var alliances = await _allianceRepository.GetAllAsync(options);
            return alliances.ToResponse();
        }

        public async Task RequestAllianceAsync(CreateAllianceRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                int requesterPartyId = GetCurrentPartyId();

                if (requesterPartyId == request.ReceiverPartyId)
                    throw new BusinessException("Auto-alianza prohibida.", "Alliance.SameParty");

                if (
                    await _allianceRepository.AllianceOrRequestExistsAsync(
                        requesterPartyId,
                        request.ReceiverPartyId
                    )
                )
                    throw new BusinessException("Alianza ya existe.", "Alliance.AlreadyExists");

                var receiverParty =
                    await _partyRepository.GetByIdAsync(request.ReceiverPartyId)
                    ?? throw new BusinessException(
                        "Receptor no encontrado.",
                        "PoliticalParty.NotFound"
                    );

                var alliance = PoliticalAlliance.Create(
                    requesterPartyId,
                    request.ReceiverPartyId,
                    receiverParty.IsActive
                );
                await _allianceRepository.AddAsync(alliance);
                await _unitOfWork.CommitAsync();

                var requesterParty = await _partyRepository.GetByIdAsync(requesterPartyId);
                var receiverPartyWithLeader = await _partyRepository.GetByIdAsync(
                    request.ReceiverPartyId,
                    p => p.LeaderAssignment!.User
                );

                if (receiverPartyWithLeader?.LeaderAssignment?.User != null)
                {
                    var model = new AllianceRequestModel(
                        requesterParty?.Name ?? "Partido Solicitante",
                        $"{receiverPartyWithLeader.LeaderAssignment.User.FirstName} {receiverPartyWithLeader.LeaderAssignment.User.LastName}",
                        _configuration["SiteSettings:AllianceUrl"]
                            ?? "https://evote360.com/admin/alliances"
                    );

                    await _emailService.SendEmailAsync(
                        receiverPartyWithLeader.LeaderAssignment.User.Email,
                        $"Nueva Solicitud de Alianza - {requesterParty?.Name}",
                        "AllianceRequest",
                        model
                    );
                }
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task AcceptAllianceAsync(int allianceId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var alliance =
                    await _allianceRepository.GetByIdAsync(
                        allianceId,
                        a => a.RequesterParty,
                        a => a.ReceiverParty,
                        a => a.RequesterParty.LeaderAssignment!.User
                    ) ?? throw new BusinessException("No encontrada.", "Alliance.NotFound");

                if (alliance.ReceiverPartyId != GetCurrentPartyId())
                    throw new BusinessException("Sin permiso.", "Alliance.AccessDenied");

                alliance.Accept(
                    alliance.RequesterParty.IsActive,
                    alliance.ReceiverParty.IsActive,
                    _dateTimeProvider.UtcNow
                );
                _allianceRepository.Update(alliance);
                await _unitOfWork.CommitAsync();

                if (alliance.RequesterParty.LeaderAssignment?.User != null)
                {
                    var model = new AllianceResponseModel(
                        alliance.ReceiverParty.Name,
                        "ACEPTADA",
                        "¡Felicidades! El pacto de alianza ha sido formalizado."
                    );

                    await _emailService.SendEmailAsync(
                        alliance.RequesterParty.LeaderAssignment.User.Email,
                        $"Pacto de Alianza Aceptado - {alliance.ReceiverParty.Name}",
                        "AllianceResponse",
                        model
                    );
                }
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task RejectAllianceAsync(int allianceId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var alliance =
                    await _allianceRepository.GetByIdAsync(
                        allianceId,
                        a => a.RequesterParty,
                        a => a.ReceiverParty,
                        a => a.RequesterParty.LeaderAssignment!.User
                    ) ?? throw new BusinessException("No encontrada.", "Alliance.NotFound");

                if (alliance.ReceiverPartyId != GetCurrentPartyId())
                    throw new BusinessException("Sin permiso.", "Alliance.AccessDenied");

                alliance.Reject();
                _allianceRepository.Update(alliance);
                await _unitOfWork.CommitAsync();

                if (alliance.RequesterParty.LeaderAssignment?.User != null)
                {
                    var model = new AllianceResponseModel(
                        alliance.ReceiverParty.Name,
                        "RECHAZADA",
                        "Lamentamos informarle que su solicitud de alianza ha sido declinada."
                    );

                    await _emailService.SendEmailAsync(
                        alliance.RequesterParty.LeaderAssignment.User.Email,
                        $"Pacto de Alianza Rechazado - {alliance.ReceiverParty.Name}",
                        "AllianceResponse",
                        model
                    );
                }
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAllianceAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var alliance =
                    await _allianceRepository.GetByIdAsync(id)
                    ?? throw new BusinessException("No encontrada.", "Alliance.NotFound");

                int partyId = GetCurrentPartyId();
                if (alliance.RequesterPartyId != partyId && alliance.ReceiverPartyId != partyId)
                    throw new BusinessException("Sin permiso.", "Alliance.AccessDenied");

                bool hasAssignments = await _allianceRepository.HasActiveAlliedAssignmentsAsync(
                    alliance.RequesterPartyId,
                    alliance.ReceiverPartyId
                );
                alliance.EnsureCanBeDeleted(hasAssignments);

                _allianceRepository.Delete(alliance);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<PoliticalPartyResponse>> GetAvailablePartiesForRequestAsync()
        {
            int myPartyId = GetCurrentPartyId();
            var myAlliances = await _allianceRepository.GetAllAsync(
                new QueryOptions<PoliticalAlliance>
                {
                    Filter = a => a.RequesterPartyId == myPartyId || a.ReceiverPartyId == myPartyId,
                    IsTracking = false,
                }
            );
            var linkedIds = myAlliances
                .Select(a =>
                    a.RequesterPartyId == myPartyId ? a.ReceiverPartyId : a.RequesterPartyId
                )
                .Concat(new[] { myPartyId })
                .ToList();

            var partyOptions = new QueryOptions<PoliticalParty>
            {
                Filter = p => p.IsActive && !linkedIds.Contains(p.Id),
                Includes = new()
                {
                    p => p.Votes,
                    p => p.CandidatePostAssignments,
                    p => p.LeaderAssignment!,
                },
                IsTracking = false,
            };
            var availableParties = await _partyRepository.GetAllAsync(partyOptions);
            return availableParties.ToResponse();
        }

        private async Task EnsureNoActiveElectionAsync()
        {
            if (await _electionRepository.AnyActiveElectionExistsAsync())
                throw new BusinessException(
                    "No se permiten gestionar alianzas mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
        }

        private int GetCurrentPartyId()
        {
            return _currentUserService.PartyId
                ?? throw new BusinessException(
                    "Sin afiliación política verificada.",
                    "Alliance.Unauthorized"
                );
        }
    }
}
