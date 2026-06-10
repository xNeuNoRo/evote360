using eVote360_Pro.Application.DTOs.Citizen.Requests;
using eVote360_Pro.Application.DTOs.Citizen.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.ValueObjects;

namespace eVote360_Pro.Application.Services
{
    
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenRepository _citizenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CitizenService(
            ICitizenRepository citizenRepository,
            IUnitOfWork unitOfWork
        )
        {
            _citizenRepository = citizenRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene todos los ciudadanos registrados con sus estados de votación.
        /// </summary>
        public async Task<IEnumerable<CitizenResponse>> GetAllAsync()
        {
            var citizens = await _citizenRepository.GetAllAsync();
            return citizens.ToResponse();
        }

        /// <summary>
        /// Obtiene un ciudadano por su ID con sus estados de votación.
        /// </summary>
        public async Task<CitizenResponse?> GetByIdAsync(int id)
        {
            var citizen = await _citizenRepository.GetByIdAsync(id);
            return citizen?.ToResponse();
        }

        /// <summary>
        /// Registra un nuevo ciudadano validando el documento de identidad mediante el Value Object.
        /// </summary>
        public async Task<CitizenResponse> CreateAsync(CreateCitizenRequest request)
        {
            // Validar que el documento de identidad no exista
            var existingCitizen = await _citizenRepository.GetByIdentityDocumentAsync(request.IdentityDocument);
            if (existingCitizen != null)
                throw new DomainException(
                    "El número de documento de identidad ya está registrado en el sistema.",
                    "Citizen.IdentityDocumentAlreadyExists"
                );

            // Validar y crear el Value Object IdentityDocument (valida Módulo 10)
            var identityDocument = IdentityDocument.Create(request.IdentityDocument);

            // Crear el ciudadano
            var citizen = Citizen.Create(
                identityDocument,
                request.FirstName,
                request.LastName,
                request.Email
            );

            // Agregar a la persistencia
            await _citizenRepository.AddAsync(citizen);
            await _unitOfWork.SaveChangesAsync();

            return citizen.ToResponse();
        }

        /// <summary>
        /// Actualiza los datos de un ciudadano existente.
        /// Antes de actualizar la cédula, valida que no haya participado en elecciones.
        /// </summary>
        public async Task<CitizenResponse> UpdateAsync(UpdateCitizenRequest request)
        {
            // Obtener el ciudadano existente
            var citizen = await _citizenRepository.GetByIdAsync(request.Id);
            if (citizen == null)
                throw new DomainException(
                    "El ciudadano especificado no existe.",
                    "Citizen.NotFound"
                );

            // Validar si la cédula cambió
            bool cedulaChanged = !string.Equals(
                citizen.IdentityDocument,
                request.IdentityDocument,
                StringComparison.OrdinalIgnoreCase
            );

            // Si la cédula cambió, verificar que no haya participado en elecciones
            if (cedulaChanged)
            {
                var hasParticipated = await _citizenRepository.HasParticipatedInAnyElectionAsync(request.Id);
                if (hasParticipated)
                    throw new DomainException(
                        "No se puede modificar el número de documento de identidad de este ciudadano porque ya participó en una elección.",
                        "Citizen.IdentityDocumentImmutable"
                    );

                // Si quiere cambiar la cédula, verificar que el nuevo documento no exista
                var existingCitizen = await _citizenRepository.GetByIdentityDocumentAsync(request.IdentityDocument);
                if (existingCitizen != null && existingCitizen.Id != request.Id)
                    throw new DomainException(
                        "El número de documento de identidad ya está registrado en el sistema.",
                        "Citizen.IdentityDocumentAlreadyExists"
                    );
            }

            // Validar y crear el Value Object IdentityDocument (valida Módulo 10)
            var identityDocument = IdentityDocument.Create(request.IdentityDocument);

            // Obtener si ha participado para pasarlo al método UpdateInformation
            var hasParticipatedInAnyElection = await _citizenRepository.HasParticipatedInAnyElectionAsync(request.Id);

            // Actualizar información del ciudadano
            citizen.UpdateInformation(
                identityDocument,
                request.FirstName,
                request.LastName,
                request.Email,
                hasParticipatedInAnyElection
            );

            // Actualizar estado de actividad
            if (!request.IsActive && citizen.IsActive)
            {
                citizen.Deactivate();
            }
            else if (request.IsActive && !citizen.IsActive)
            {
                citizen.Activate();
            }

            // Persistir cambios
            await _unitOfWork.SaveChangesAsync();

            return citizen.ToResponse();
        }

        /// <summary>
        /// Activa o desactiva un ciudadano.
        /// </summary>
        public async Task ToggleStatusAsync(int id, bool activate)
        {
            // Obtener el ciudadano
            var citizen = await _citizenRepository.GetByIdAsync(id);
            if (citizen == null)
                throw new DomainException(
                    "El ciudadano especificado no existe.",
                    "Citizen.NotFound"
                );

            // Cambiar estado
            if (!activate)
            {
                citizen.Deactivate();
            }
            else
            {
                citizen.Activate();
            }

            // Persistir cambios
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
