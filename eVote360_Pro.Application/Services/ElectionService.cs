using eVote360_Pro.Application.DTOs.Election.Requests;
using eVote360_Pro.Application.DTOs.Election.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    
    public class ElectionService : IElectionService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ElectionService(
            IElectionRepository electionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ElectionResponse>> GetAllAsync()
        {
            var elections = await _electionRepository.GetAllAsync();
            return elections.ToResponse();
        }

        public async Task<ElectionResponse?> GetByIdAsync(Guid id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            return election?.ToResponse();
        }

        public async Task<ElectionResponse?> GetActiveElectionAsync()
        {
            var election = await _electionRepository.GetActiveElectionAsync();
            return election?.ToResponse();
        }

        public async Task<ElectionResponse> CreateAsync(CreateElectionRequest request)
        {
            if (await _electionRepository.AnyActiveElectionExistsAsync())
                throw new DomainException(
                    "No se puede crear una nueva elección mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );

            var election = Election.Create(
                request.Name,
                request.RealizationDate,
                false
            );

            await _electionRepository.AddAsync(election);
            await _unitOfWork.SaveChangesAsync();

            return election.ToResponse();
        }

        public async Task<ElectionResponse> UpdateAsync(UpdateElectionRequest request)
        {
            var election = await _electionRepository.GetByIdAsync(request.Id);
            if (election == null)
                throw new DomainException(
                    "La elección especificada no existe.",
                    "Election.NotFound"
                );

            election.UpdateInformation(request.Name, request.RealizationDate);
            await _unitOfWork.SaveChangesAsync();

            return election.ToResponse();
        }

        public async Task ActivateAsync(Guid id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new DomainException(
                    "La elección especificada no existe.",
                    "Election.NotFound"
                );

            var hasAnotherActiveElection = await _electionRepository.AnyActiveElectionExistsAsync();
            var (hasActivePositions, hasMinimumParties) = await _electionRepository.GetElectoralBasicsStatusAsync();
            var missingParties = await _electionRepository.GetPartiesWithMissingCandidatesAsync();

            election.Activate(
                hasAnotherActiveElection,
                hasActivePositions,
                hasMinimumParties,
                missingParties
            );

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task FinishAsync(Guid id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new DomainException(
                    "La elección especificada no existe.",
                    "Election.NotFound"
                );

            election.Finish();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                throw new DomainException(
                    "La elección especificada no existe.",
                    "Election.NotFound"
                );

            election.Deactivate();
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
