using eVote360_Pro.Application.DTOs.ElectivePosition.Requests;
using eVote360_Pro.Application.DTOs.ElectivePosition.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona los cargos disponibles para los procesos de votación.
    /// </summary>
    public interface IElectivePositionService
    {
        /// <summary>
        /// Obtiene todos los cargos registrados.
        /// </summary>
        Task<IEnumerable<ElectivePositionResponse>> GetAllAsync();

        /// <summary>
        /// Obtiene un cargo por su ID.
        /// </summary>
        Task<ElectivePositionResponse?> GetByIdAsync(int id);

        /// <summary>
        /// Registra un nuevo cargo.
        /// </summary>
        Task<ElectivePositionResponse> CreateAsync(CreateElectivePositionRequest request);

        /// <summary>
        /// Actualiza los datos de un cargo existente.
        /// </summary>
        Task<ElectivePositionResponse> UpdateAsync(UpdateElectivePositionRequest request);

        /// <summary>
        /// Alterna el estado de un cargo.
        /// </summary>
        Task ToggleStatusAsync(int id, bool activate);
    }
}
