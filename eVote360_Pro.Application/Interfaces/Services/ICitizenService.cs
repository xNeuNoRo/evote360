using eVote360_Pro.Application.DTOs.Citizen.Requests;
using eVote360_Pro.Application.DTOs.Citizen.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona el padrón electoral del sistema.
    /// </summary>
    public interface ICitizenService
    {
        /// <summary>
        /// Obtiene todos los ciudadanos registrados.
        /// </summary>
        Task<IEnumerable<CitizenResponse>> GetAllAsync();

        /// <summary>
        /// Obtiene un ciudadano por su ID.
        /// </summary>
        Task<CitizenResponse?> GetByIdAsync(int id);

        /// <summary>
        /// Registra un nuevo ciudadano.
        /// </summary>
        Task<CitizenResponse> CreateAsync(CreateCitizenRequest request);

        /// <summary>
        /// Actualiza los datos de un ciudadano existente.
        /// </summary>
        Task<CitizenResponse> UpdateAsync(UpdateCitizenRequest request);

        /// <summary>
        /// Activa o desactiva un ciudadano.
        /// </summary>
        Task ToggleStatusAsync(int id, bool activate);
    }
}
