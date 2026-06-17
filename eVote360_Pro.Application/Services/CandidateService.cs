using eVote360_Pro.Application.DTOs.Candidate.Requests;
using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de candidatos.
    /// </summary>
    public class CandidateService : ICandidateService
    {
        private readonly ICandidatesRepository _candidateRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public CandidateService(
            ICandidatesRepository candidateRepository,
            IElectionRepository electionRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IFileService fileService
        )
        {
            _candidateRepository = candidateRepository;
            _electionRepository = electionRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<CandidateResponse>> GetAllAsync()
        {
            var options = new QueryOptions<Candidate>
            {
                Includes = new() { c => c.OriginalParty, c => c.Votes },
                IsTracking = false,
            };

            if (_currentUserService.Role == SystemRoles.PoliticalLeader)
                options.Filter = c => c.OriginalPartyId == GetCurrentPartyId();

            var candidates = await _candidateRepository.GetAllAsync(options);
            return candidates.ToResponse();
        }

        public async Task<CandidateResponse?> GetByIdAsync(int id)
        {
            var candidate = await _candidateRepository.GetByIdAsync(
                id,
                c => c.OriginalParty,
                c => c.Votes
            );

            if (
                candidate != null
                && _currentUserService.Role == SystemRoles.PoliticalLeader
                && candidate.OriginalPartyId != GetCurrentPartyId()
            )
                throw new BusinessException("Sin permiso.", "Candidate.AccessDenied");

            return candidate?.ToResponse();
        }

        public async Task<CandidateResponse> CreateAsync(CreateCandidateRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                int partyId =
                    _currentUserService.Role == SystemRoles.PoliticalLeader
                        ? GetCurrentPartyId()
                        : (
                            request.OriginalPartyId
                            ?? throw new ValidationBusinessException(
                                nameof(request.OriginalPartyId),
                                "Partido requerido.",
                                "Candidate.PartyRequired"
                            )
                        );

                if (!_fileService.IsImageValid(request.PhotoFile))
                    throw new ValidationBusinessException(
                        nameof(request.PhotoFile),
                        "La foto no es válida o es muy pesada.",
                        "General.InvalidFile"
                    );

                string photoPath = await _fileService.UploadFileAsync(
                    request.PhotoFile,
                    "candidates"
                );

                var candidate = Candidate.Create(
                    request.FirstName,
                    request.LastName,
                    photoPath,
                    partyId
                );
                await _candidateRepository.AddAsync(candidate);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(candidate.Id) ?? candidate.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<CandidateResponse> UpdateAsync(UpdateCandidateRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var candidate =
                    await _candidateRepository.GetByIdAsync(
                        request.Id,
                        c => c.OriginalParty,
                        c => c.Votes
                    )
                    ?? throw new BusinessException(
                        "Candidato no encontrado.",
                        "Candidate.NotFound"
                    );

                if (
                    _currentUserService.Role == SystemRoles.PoliticalLeader
                    && candidate.OriginalPartyId != GetCurrentPartyId()
                )
                    throw new BusinessException("Sin permiso.", "Candidate.AccessDenied");

                bool hasParticipated = await _candidateRepository.HasParticipatedInAnyElectionAsync(
                    candidate.Id
                );
                string? newPhotoPath = null;
                string? oldPhotoPath = candidate.PhotoPath;

                if (request.PhotoFile != null && !hasParticipated)
                {
                    if (!_fileService.IsImageValid(request.PhotoFile))
                        throw new ValidationBusinessException(
                            nameof(request.PhotoFile),
                            "La foto no es válida o es muy pesada.",
                            "General.InvalidFile"
                        );

                    newPhotoPath = await _fileService.UploadFileAsync(
                        request.PhotoFile,
                        "candidates"
                    );
                }

                candidate.UpdateInformation(
                    request.FirstName,
                    request.LastName,
                    newPhotoPath,
                    hasParticipated
                );

                if (candidate.IsActive != request.IsActive)
                {
                    if (request.IsActive)
                        candidate.Activate();
                    else
                        candidate.Deactivate(
                            await _candidateRepository.IsAssignedToAnyPostAsync(candidate.Id)
                        );
                }

                _candidateRepository.Update(candidate);
                await _unitOfWork.CommitAsync();

                if (newPhotoPath != null)
                    _fileService.DeleteFile(oldPhotoPath);

                return candidate.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task ToggleStatusAsync(int id, bool activate)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var candidate =
                    await _candidateRepository.GetByIdAsync(id)
                    ?? throw new BusinessException(
                        "Candidato no encontrado.",
                        "Candidate.NotFound"
                    );

                if (
                    _currentUserService.Role == SystemRoles.PoliticalLeader
                    && candidate.OriginalPartyId != GetCurrentPartyId()
                )
                    throw new BusinessException("Sin permiso.", "Candidate.AccessDenied");

                if (activate)
                    candidate.Activate();
                else
                    candidate.Deactivate(await _candidateRepository.IsAssignedToAnyPostAsync(id));

                _candidateRepository.Update(candidate);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task EnsureNoActiveElectionAsync()
        {
            if (await _electionRepository.AnyActiveElectionExistsAsync())
                throw new BusinessException(
                    "No se permiten cambios en los candidatos mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
        }

        private int GetCurrentPartyId()
        {
            return _currentUserService.PartyId
                ?? throw new BusinessException(
                    "Sin afiliación política verificada.",
                    "Candidate.Unauthorized"
                );
        }
    }
}
