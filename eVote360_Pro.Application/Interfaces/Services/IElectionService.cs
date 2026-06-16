using eVote360_Pro.Application.DTOs.Election.Requests;
using eVote360_Pro.Application.DTOs.Election.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Gestiona el ciclo de vida de los procesos electorales.
    /// </summary>
    public interface IElectionService
    {
        /// <summary>
        /// Obtiene todas las elecciones registradas.
        /// </summary>
        Task<IEnumerable<ElectionResponse>> GetAllAsync();

        /// <summary>
        /// Obtiene una elección por su ID.
        /// </summary>
        Task<ElectionResponse?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene la elección que está actualmente capturando votos.
        /// </summary>
        Task<ElectionResponse?> GetActiveElectionAsync();

        /// <summary>
        /// Registra una nueva elección en estado pendiente, lista para configurar la boleta y luego activar.
        /// </summary>
        Task<ElectionResponse> CreateAsync(CreateElectionRequest request);

        /// <summary>
        /// Actualiza los datos de una elección en estado pendiente.
        /// </summary>
        Task<ElectionResponse> UpdateAsync(UpdateElectionRequest request);

        /// <summary>
        /// Activa la elección validando la configuración completa de la boleta de todos los partidos.
        /// </summary>
        Task ActivateAsync(Guid id);

        /// <summary>
        /// Finaliza la elección, habilitando la visualización de resultados.
        /// </summary>
        Task FinishAsync(Guid id);

        /// <summary>
        /// Elimina lógicamente una elección en estado pendiente.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
