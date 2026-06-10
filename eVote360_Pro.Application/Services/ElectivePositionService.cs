using eVote360_Pro.Application.DTOs.ElectivePosition.Requests;
using eVote360_Pro.Application.DTOs.ElectivePosition.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    public class ElectivePositionService : IElectivePositionService
    {
        private readonly IElectivePositionsRepository _electivePositionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ElectivePositionService(
            IElectivePositionsRepository electivePositionRepository,
            IUnitOfWork unitOfWork)
        {
            _electivePositionRepository = electivePositionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ElectivePositionResponse>> GetAllAsync()
        {
            var positions = await _electivePositionRepository.GetAllAsync();
            return positions.ToResponse();
        }

        public async Task<ElectivePositionResponse?> GetByIdAsync(int id)
        {
            var position = await _electivePositionRepository.GetByIdAsync(id);
            return position?.ToResponse();
        }

        public async Task<ElectivePositionResponse> CreateAsync(CreateElectivePositionRequest request)
        {
            if (await _electivePositionRepository.ExistsByNameAsync(request.Name))
            {
                throw new DomainException("Ya existe un cargo electivo registrado con este nombre.", "ElectivePosition.DuplicateName");
            }

            var position = ElectivePosition.Create(request.Name, request.Description);
            await _electivePositionRepository.AddAsync(position);
            await _unitOfWork.SaveChangesAsync();

            return position.ToResponse();
        }

        public async Task DeleteAsync(int id)
        {
            var position = await _electivePositionRepository.GetByIdAsync(id);
            if (position == null)
            {
                throw new DomainException("El cargo electivo especificado no existe.", "ElectivePosition.NotFound");
            }

            _electivePositionRepository.Delete(position);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleStatusAsync(int id, bool activate)
        {
            var position = await _electivePositionRepository.GetByIdAsync(id);
            if (position == null)
            {
                throw new DomainException("El cargo electivo especificado no existe.", "ElectivePosition.NotFound");
            }

            if (activate)
            {
                position.Activate();
            }
            else
            {
                var hasActiveCandidates = await _electivePositionRepository.HasActiveCandidatesAsync(id);
                position.Deactivate(hasActiveCandidates);
            }

            _electivePositionRepository.Update(position);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ElectivePositionResponse> UpdateAsync(UpdateElectivePositionRequest request)
        {
            var position = await _electivePositionRepository.GetByIdAsync(request.Id);
            if (position == null)
            {
                throw new DomainException("El cargo electivo especificado no existe.", "ElectivePosition.NotFound");
            }

            if (await _electivePositionRepository.ExistsByNameAsync(request.Name, request.Id))
            {
                throw new DomainException("Ya existe otro cargo electivo registrado con este nombre.", "ElectivePosition.DuplicateName");
            }

            var wasUsed = await _electivePositionRepository.WasUsedInAnyElectionAsync(request.Id);
            position.UpdateInformation(request.Name, request.Description, wasUsed);

            _electivePositionRepository.Update(position);
            await _unitOfWork.SaveChangesAsync();

            return position.ToResponse();
        }
    }
}
