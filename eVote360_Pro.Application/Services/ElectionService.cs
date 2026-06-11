using eVote360_Pro.Application.DTOs.Election.Requests;
using eVote360_Pro.Application.DTOs.Election.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de elecciones.
    /// </summary>
    public class ElectionService : IElectionService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ElectionService(IElectionRepository electionRepository, IUnitOfWork unitOfWork)
        {
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ElectionResponse>> GetAllAsync()
        {
            var options = new QueryOptions<Election>
            {
                IsTracking = false,
                OrderBy = q => q.OrderByDescending(e => e.RealizationDate),
            };

            var elections = await _electionRepository.GetAllAsync(options);
            return elections.ToResponse();
        }

        public async Task<ElectionResponse?> GetByIdAsync(Guid id)
        {
            var election = await _electionRepository.GetByIdAsync(id);
            if (election == null)
                return null;

            bool canActivate = false;
            IEnumerable<string> missingParties = Enumerable.Empty<string>();

            if (election.Status == ElectionStatus.Pending)
            {
                var hasAnotherActiveElection =
                    await _electionRepository.AnyActiveElectionExistsAsync();
                var (hasActivePositions, hasMinimumParties) =
                    await _electionRepository.GetElectoralBasicsStatusAsync();
                var missingPartiesList =
                    await _electionRepository.GetPartiesWithMissingCandidatesAsync();

                missingParties = missingPartiesList;
                canActivate =
                    !hasAnotherActiveElection
                    && hasActivePositions
                    && hasMinimumParties
                    && !missingParties.Any();
            }

            return election.ToResponse(canActivate, missingParties);
        }

        public async Task<ElectionResponse?> GetActiveElectionAsync()
        {
            var election = await _electionRepository.GetActiveElectionAsync();
            return election?.ToResponse();
        }

        public async Task<ElectionResponse> CreateAsync(CreateElectionRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (await _electionRepository.AnyActiveElectionExistsAsync())
                {
                    throw new BusinessException(
                        "No se puede crear una nueva elección mientras exista una elección activa.",
                        "Election.ActiveAlreadyExists"
                    );
                }

                var election = Election.Create(request.Name, request.RealizationDate, false);

                await _electionRepository.AddAsync(election);
                await _unitOfWork.CommitAsync();

                return election.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ElectionResponse> UpdateAsync(UpdateElectionRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var election =
                    await _electionRepository.GetByIdAsync(request.Id)
                    ?? throw new BusinessException(
                        "La elección especificada no existe.",
                        "Election.NotFound"
                    );

                election.UpdateInformation(request.Name, request.RealizationDate);

                _electionRepository.Update(election);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(election.Id) ?? election.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task ActivateAsync(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var election =
                    await _electionRepository.GetByIdAsync(id)
                    ?? throw new BusinessException(
                        "La elección especificada no existe.",
                        "Election.NotFound"
                    );

                var hasAnotherActiveElection =
                    await _electionRepository.AnyActiveElectionExistsAsync();
                var (hasActivePositions, hasMinimumParties) =
                    await _electionRepository.GetElectoralBasicsStatusAsync();
                var missingParties =
                    await _electionRepository.GetPartiesWithMissingCandidatesAsync();

                election.Activate(
                    hasAnotherActiveElection,
                    hasActivePositions,
                    hasMinimumParties,
                    missingParties
                );

                _electionRepository.Update(election);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task FinishAsync(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var election =
                    await _electionRepository.GetByIdAsync(id)
                    ?? throw new BusinessException(
                        "La elección especificada no existe.",
                        "Election.NotFound"
                    );

                election.Finish();

                _electionRepository.Update(election);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var election =
                    await _electionRepository.GetByIdAsync(id)
                    ?? throw new BusinessException(
                        "La elección especificada no existe.",
                        "Election.NotFound"
                    );

                election.Deactivate();

                _electionRepository.Update(election);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
