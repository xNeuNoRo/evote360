using eVote360_Pro.Application.DTOs.PoliticalParty.Requests;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Shared.Interfaces.Storage;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de partidos.
    /// </summary>
    public class PoliticalPartyService : IPoliticalPartyService
    {
        private readonly IPoliticalPartiesRepository _partyRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public PoliticalPartyService(
            IPoliticalPartiesRepository partyRepository,
            IElectionRepository electionRepository,
            IUnitOfWork unitOfWork,
            IFileService fileService
        )
        {
            _partyRepository = partyRepository;
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<PoliticalPartyResponse>> GetAllAsync()
        {
            var options = new QueryOptions<PoliticalParty>
            {
                Includes = new()
                {
                    p => p.Votes,
                    p => p.CandidatePostAssignments,
                    p => p.LeaderAssignment!,
                },
                IsTracking = false,
            };
            var parties = await _partyRepository.GetAllAsync(options);
            return parties.ToResponse();
        }

        public async Task<PoliticalPartyResponse?> GetByIdAsync(int id)
        {
            var party = await _partyRepository.GetByIdAsync(
                id,
                p => p.Votes,
                p => p.CandidatePostAssignments,
                p => p.LeaderAssignment!
            );
            return party?.ToResponse();
        }

        public async Task<PoliticalPartyResponse> CreateAsync(CreatePoliticalPartyRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                if (await _partyRepository.ExistsByNameAsync(request.Name))
                    throw new ValidationBusinessException(
                        nameof(request.Name),
                        "Nombre en uso.",
                        "PoliticalParty.DuplicateName"
                    );

                if (await _partyRepository.ExistsByAcronymAsync(request.Acronym))
                    throw new ValidationBusinessException(
                        nameof(request.Acronym),
                        "Siglas en uso.",
                        "PoliticalParty.DuplicateAcronym"
                    );

                if (!_fileService.IsImageValid(request.LogoFile))
                    throw new ValidationBusinessException(
                        nameof(request.LogoFile),
                        "El logo no es válido o es muy pesado.",
                        "General.InvalidFile"
                    );

                string logoPath = await _fileService.UploadFileAsync(request.LogoFile, "parties");

                var party = PoliticalParty.Create(
                    request.Name,
                    request.Acronym,
                    logoPath,
                    request.Description,
                    request.IsActive
                );

                await _partyRepository.AddAsync(party);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(party.Id) ?? party.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PoliticalPartyResponse> UpdateAsync(UpdatePoliticalPartyRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var party =
                    await _partyRepository.GetByIdAsync(request.Id)
                    ?? throw new BusinessException(
                        "Partido no encontrado.",
                        "PoliticalParty.NotFound"
                    );

                if (await _partyRepository.ExistsByNameAsync(request.Name, request.Id))
                    throw new ValidationBusinessException(
                        nameof(request.Name),
                        "Nombre en uso.",
                        "PoliticalParty.DuplicateName"
                    );

                if (await _partyRepository.ExistsByAcronymAsync(request.Acronym, request.Id))
                    throw new ValidationBusinessException(
                        nameof(request.Acronym),
                        "Siglas en uso.",
                        "PoliticalParty.DuplicateAcronym"
                    );

                bool hasParticipated = await _partyRepository.WasUsedInAnyElectionAsync(party.Id);
                string? newLogoPath = null;
                string? oldLogoPath = party.LogoPath;

                if (request.LogoFile != null && !hasParticipated)
                {
                    if (!_fileService.IsImageValid(request.LogoFile))
                        throw new ValidationBusinessException(
                            nameof(request.LogoFile),
                            "El logo no es válido o es muy pesado.",
                            "General.InvalidFile"
                        );

                    newLogoPath = await _fileService.UploadFileAsync(request.LogoFile, "parties");
                }

                party.UpdateInformation(
                    request.Name,
                    request.Acronym,
                    newLogoPath,
                    request.Description,
                    hasParticipated
                );

                if (party.IsActive != request.IsActive)
                {
                    if (request.IsActive)
                        party.Activate();
                    else
                        party.Deactivate(
                            await _partyRepository.HasActiveCandidatesAsync(party.Id),
                            await _partyRepository.HasActiveLeaderAsync(party.Id)
                        );
                }

                _partyRepository.Update(party);
                await _unitOfWork.CommitAsync();

                if (newLogoPath != null)
                    _fileService.DeleteFile(oldLogoPath);

                return await GetByIdAsync(party.Id) ?? party.ToResponse();
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

                var party =
                    await _partyRepository.GetByIdAsync(id)
                    ?? throw new BusinessException(
                        "Partido no encontrado.",
                        "PoliticalParty.NotFound"
                    );

                if (activate)
                    party.Activate();
                else
                    party.Deactivate(
                        await _partyRepository.HasActiveCandidatesAsync(id),
                        await _partyRepository.HasActiveLeaderAsync(id)
                    );

                _partyRepository.Update(party);
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
                    "No se permiten cambios en los partidos mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
        }
    }
}
