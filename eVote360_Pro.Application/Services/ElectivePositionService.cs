using eVote360_Pro.Application.DTOs.ElectivePosition.Requests;
using eVote360_Pro.Application.DTOs.ElectivePosition.Responses;
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
    /// Implementación del servicio de puestos electivos.
    /// </summary>
    public class ElectivePositionService : IElectivePositionService
    {
        private readonly IElectivePositionsRepository _positionRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ElectivePositionService(
            IElectivePositionsRepository positionRepository,
            IElectionRepository electionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _positionRepository = positionRepository;
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ElectivePositionResponse>> GetAllAsync()
        {
            var options = new QueryOptions<ElectivePosition>
            {
                Includes = new() { p => p.Votes, p => p.CandidatePostAssignments },
                IsTracking = false,
            };
            var positions = await _positionRepository.GetAllAsync(options);
            return positions.ToResponse();
        }

        public async Task<ElectivePositionResponse?> GetByIdAsync(int id)
        {
            var position = await _positionRepository.GetByIdAsync(
                id,
                p => p.Votes,
                p => p.CandidatePostAssignments
            );
            return position?.ToResponse();
        }

        public async Task<ElectivePositionResponse> CreateAsync(
            CreateElectivePositionRequest request
        )
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                if (await _positionRepository.ExistsByNameAsync(request.Name))
                    throw new ValidationBusinessException(
                        nameof(request.Name),
                        "Nombre en uso.",
                        "ElectivePosition.DuplicateName"
                    );

                var position = ElectivePosition.Create(request.Name, request.Description);
                await _positionRepository.AddAsync(position);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(position.Id) ?? position.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ElectivePositionResponse> UpdateAsync(
            UpdateElectivePositionRequest request
        )
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var position =
                    await _positionRepository.GetByIdAsync(
                        request.Id,
                        p => p.Votes,
                        p => p.CandidatePostAssignments
                    )
                    ?? throw new BusinessException(
                        "Puesto no encontrado.",
                        "ElectivePosition.NotFound"
                    );

                if (await _positionRepository.ExistsByNameAsync(request.Name, request.Id))
                    throw new ValidationBusinessException(
                        nameof(request.Name),
                        "Nombre en uso.",
                        "ElectivePosition.DuplicateName"
                    );

                bool wasUsed = await _positionRepository.WasUsedInAnyElectionAsync(request.Id);
                position.UpdateInformation(request.Name, request.Description, wasUsed);

                if (position.IsActive != request.IsActive)
                {
                    if (request.IsActive)
                        position.Activate();
                    else
                        position.Deactivate(
                            await _positionRepository.HasActiveCandidatesAsync(position.Id)
                        );
                }

                _positionRepository.Update(position);
                await _unitOfWork.CommitAsync();

                return position.ToResponse();
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

                var position =
                    await _positionRepository.GetByIdAsync(id)
                    ?? throw new BusinessException(
                        "Puesto no encontrado.",
                        "ElectivePosition.NotFound"
                    );

                if (activate)
                    position.Activate();
                else
                    position.Deactivate(await _positionRepository.HasActiveCandidatesAsync(id));

                _positionRepository.Update(position);
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
                    "No se permiten cambios en los puestos mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
        }
    }
}
