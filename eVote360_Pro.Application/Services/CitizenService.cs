using eVote360_Pro.Application.DTOs.Citizen.Requests;
using eVote360_Pro.Application.DTOs.Citizen.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.ValueObjects;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de ciudadanos.
    /// </summary>
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CitizenService(
            ICitizenRepository citizenRepository,
            IElectionRepository electionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _citizenRepository = citizenRepository;
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CitizenResponse>> GetAllAsync()
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();
            var options = new QueryOptions<Citizen>
            {
                Includes = new List<System.Linq.Expressions.Expression<Func<Citizen, object>>>
                {
                    c => c.Participations,
                },
                IsTracking = false,
            };

            var citizens = await _citizenRepository.GetAllAsync(options);
            return citizens.ToResponse(activeElection?.Id);
        }

        public async Task<CitizenResponse?> GetByIdAsync(int id)
        {
            var activeElection = await _electionRepository.GetActiveElectionAsync();
            var citizen = await _citizenRepository.GetByIdAsync(id, c => c.Participations);
            return citizen?.ToResponse(activeElection?.Id);
        }

        public async Task<CitizenResponse> CreateAsync(CreateCitizenRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var existingCitizen = await _citizenRepository.GetByIdentityDocumentAsync(
                    request.IdentityDocument
                );
                if (existingCitizen != null)
                {
                    throw new ValidationBusinessException(
                        nameof(request.IdentityDocument),
                        "El documento ya está registrado.",
                        "Citizen.IdentityDocumentAlreadyExists"
                    );
                }

                var citizen = Citizen.Create(
                    IdentityDocument.Create(request.IdentityDocument),
                    request.FirstName,
                    request.LastName,
                    request.Email
                );

                await _citizenRepository.AddAsync(citizen);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(citizen.Id) ?? citizen.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<CitizenResponse> UpdateAsync(UpdateCitizenRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var citizen =
                    await _citizenRepository.GetByIdAsync(request.Id, c => c.Participations)
                    ?? throw new BusinessException("Ciudadano no encontrado.", "Citizen.NotFound");

                var identityDocument = IdentityDocument.Create(request.IdentityDocument);

                // Validación de unicidad si cambia la cédula
                if (citizen.IdentityDocument != identityDocument.Value)
                {
                    var existing = await _citizenRepository.GetByIdentityDocumentAsync(
                        identityDocument.Value
                    );
                    if (existing != null && existing.Id != request.Id)
                        throw new ValidationBusinessException(
                            nameof(request.IdentityDocument),
                            "Cédula en uso.",
                            "Citizen.IdentityDocumentAlreadyExists"
                        );
                }

                citizen.UpdateInformation(
                    identityDocument,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    citizen.Participations.Any()
                );

                if (request.IsActive != citizen.IsActive)
                {
                    if (request.IsActive)
                        citizen.Activate();
                    else
                        citizen.Deactivate();
                }

                _citizenRepository.Update(citizen);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(citizen.Id) ?? citizen.ToResponse();
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

                var citizen =
                    await _citizenRepository.GetByIdAsync(id)
                    ?? throw new BusinessException("Ciudadano no encontrado.", "Citizen.NotFound");

                if (activate)
                    citizen.Activate();
                else
                    citizen.Deactivate();

                _citizenRepository.Update(citizen);
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
            {
                throw new BusinessException(
                    "No se permiten cambios en el padrón electoral mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
            }
        }
    }
}
