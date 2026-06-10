using System.Security.Claims;
using eVote360_Pro.Application.DTOs.Candidate.Requests;
using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Shared.Interfaces.Storage;
using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidatesRepository _candidateRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public CandidateService(
            ICandidatesRepository candidateRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IFileService fileService
        )
        {
            _candidateRepository = candidateRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<CandidateResponse>> GetAllAsync()
        {
            var options = new QueryOptions<Candidate>
            {
                Includes = new() { c => c.OriginalParty, c => c.Votes }
            };

            // Si el usuario es dirigente, filtra solo los de su partido.
            if (IsPoliticalLeader())
            {
                int partyId = GetCurrentPartyId();
                options.Filter = c => c.OriginalPartyId == partyId;
            }

            var candidates = await _candidateRepository.GetAllAsync(options);
            return candidates.ToResponse();
        }

        public async Task<CandidateResponse?> GetByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id, c => c.OriginalParty, c => c.Votes);

            if (candidate != null && IsPoliticalLeader() && candidate.OriginalPartyId != GetCurrentPartyId())
            {
                throw new DomainException("No tiene permiso para ver este candidato.", "Candidate.AccessDenied");
            }

            return candidate?.ToResponse();
        }

        public async Task<CandidateResponse> CreateAsync(CreateCandidateRequest request)
        {
            int partyId;
            if (IsPoliticalLeader())
            {
                partyId = GetCurrentPartyId();
            }
            else
            {
                partyId = request.OriginalPartyId ?? throw new DomainException("El partido político de origen es requerido.", "Candidate.PartyRequired");
            }

            // Gestión de la fotografía
            string photoPath = await _fileService.UploadFileAsync(request.PhotoFile, "candidates");

            var candidate = Candidate.Create(
                request.FirstName,
                request.LastName,
                photoPath,
                partyId
            );

            await _candidateRepository.AddAsync(candidate);
            await _unitOfWork.SaveChangesAsync();

            return candidate.ToResponse();
        }

        public async Task<CandidateResponse> UpdateAsync(UpdateCandidateRequest request)
        {
            var candidate = await _candidateRepository.GetByIdAsync(request.Id)
                            ?? throw new DomainException("El candidato especificado no existe.", "Candidate.NotFound");

            if (IsPoliticalLeader() && candidate.OriginalPartyId != GetCurrentPartyId())
            {
                throw new DomainException("No tiene permiso para editar este candidato.", "Candidate.AccessDenied");
            }

            string? photoPath = null;
            if (request.PhotoFile != null)
            {
                photoPath = await _fileService.UploadFileAsync(request.PhotoFile, "candidates");
            }

            bool hasParticipated = await _candidateRepository.HasParticipatedInAnyElectionAsync(candidate.Id);

            candidate.UpdateInformation(
                request.FirstName,
                request.LastName,
                photoPath,
                hasParticipated
            );

            if (request.IsActive)
            {
                candidate.Activate();
            }
            else
            {
                bool hasActiveAssignment = await _candidateRepository.IsAssignedToAnyPostAsync(candidate.Id);
                candidate.Deactivate(hasActiveAssignment);
            }

            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();

            return candidate.ToResponse();
        }

        public async Task ToggleStatusAsync(int id, bool activate)
        {
            var candidate = await _candidateRepository.GetByIdAsync(id)
                            ?? throw new DomainException("El candidato especificado no existe.", "Candidate.NotFound");

            if (IsPoliticalLeader() && candidate.OriginalPartyId != GetCurrentPartyId())
            {
                throw new DomainException("No tiene permiso para gestionar este candidato.", "Candidate.AccessDenied");
            }

            if (activate)
            {
                candidate.Activate();
            }
            else
            {
                bool hasActiveAssignment = await _candidateRepository.IsAssignedToAnyPostAsync(candidate.Id);
                candidate.Deactivate(hasActiveAssignment);
            }

            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();
        }

        private bool IsPoliticalLeader()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(SystemRoles.PoliticalLeader) ?? false;
        }

        private int GetCurrentPartyId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var partyIdClaim = user?.FindFirst("party_id")?.Value;

            if (string.IsNullOrEmpty(partyIdClaim) || !int.TryParse(partyIdClaim, out int partyId))
            {
                throw new DomainException("No se pudo identificar el partido político del usuario logueado.", "Candidate.Unauthorized");
            }

            return partyId;
        }
    }
}
