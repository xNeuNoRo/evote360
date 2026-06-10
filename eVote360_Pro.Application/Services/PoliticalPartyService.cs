using eVote360_Pro.Application.DTOs.PoliticalParty.Requests;
using eVote360_Pro.Application.DTOs.PoliticalParty.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Shared.Interfaces.Storage;

namespace eVote360_Pro.Application.Services
{
    public class PoliticalPartyService : IPoliticalPartyService
    {
        private readonly IPoliticalPartiesRepository _partyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public PoliticalPartyService(
            IPoliticalPartiesRepository partyRepository,
            IUnitOfWork unitOfWork,
            IFileService fileService
        )
        {
            _partyRepository = partyRepository;
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<IEnumerable<PoliticalPartyResponse>> GetAllAsync()
        {
            var parties = await _partyRepository.GetAllAsync();
            return parties.ToResponse();
        }

        public async Task<PoliticalPartyResponse?> GetByIdAsync(int id)
        {
            var party = await _partyRepository.GetByIdAsync(id);
            return party?.ToResponse();
        }

        public async Task<PoliticalPartyResponse> CreateAsync(CreatePoliticalPartyRequest request)
        {
            if (await _partyRepository.ExistsByNameAsync(request.Name))
                throw new DomainException(
                    "Ya existe un partido con ese nombre.",
                    "PoliticalParty.DuplicateName"
                );

            if (await _partyRepository.ExistsByAcronymAsync(request.Acronym))
                throw new DomainException(
                    "Ya existe un partido con esas siglas.",
                    "PoliticalParty.DuplicateAcronym"
                );

            string logoPath = await _fileService.UploadFileAsync(request.LogoFile, "parties");

            var party = PoliticalParty.Create(
                request.Name,
                request.Acronym,
                logoPath,
                request.Description
            );

            await _partyRepository.AddAsync(party);
            await _unitOfWork.SaveChangesAsync();

            return party.ToResponse();
        }

        public async Task<PoliticalPartyResponse> UpdateAsync(UpdatePoliticalPartyRequest request)
        {
            var party =
                await _partyRepository.GetByIdAsync(request.Id)
                ?? throw new DomainException(
                    "El partido político no existe.",
                    "PoliticalParty.NotFound"
                );

            if (await _partyRepository.ExistsByNameAsync(request.Name, request.Id))
                throw new DomainException(
                    "Ya existe un partido con ese nombre.",
                    "PoliticalParty.DuplicateName"
                );

            if (await _partyRepository.ExistsByAcronymAsync(request.Acronym, request.Id))
                throw new DomainException(
                    "Ya existe un partido con esas siglas.",
                    "PoliticalParty.DuplicateAcronym"
                );

            string? logoPath = null;
            if (request.LogoFile != null)
            {
                logoPath = await _fileService.UploadFileAsync(request.LogoFile, "parties");
            }

            bool hasParticipated = await _partyRepository.WasUsedInAnyElectionAsync(party.Id);

            party.UpdateInformation(
                request.Name,
                request.Acronym,
                logoPath,
                request.Description,
                hasParticipated
            );

            _partyRepository.Update(party);
            await _unitOfWork.SaveChangesAsync();

            return party.ToResponse();
        }

        public async Task ToggleStatusAsync(int id, bool activate)
        {
            var party =
                await _partyRepository.GetByIdAsync(id)
                ?? throw new DomainException(
                    "El partido político no existe.",
                    "PoliticalParty.NotFound"
                );

            if (activate)
            {
                party.Activate();
            }
            else
            {
                bool hasActiveCandidates = await _partyRepository.HasActiveCandidatesAsync(id);
                bool hasAssignedLeader = await _partyRepository.HasActiveLeaderAsync(id);
                party.Deactivate(hasActiveCandidates, hasAssignedLeader);
            }

            _partyRepository.Update(party);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
