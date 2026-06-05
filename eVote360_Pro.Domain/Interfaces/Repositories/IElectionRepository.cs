using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y gestión de los procesos electorales.
    /// Encapsula las reglas de activación y ciclo de vida de las elecciones.
    /// </summary>
    public interface IElectionRepository : IGenericRepository<Election, Guid>
    {
        /// <summary>
        /// Obtiene la elección que se encuentra actualmente en estado 'Activa'.
        /// Solo puede existir una elección activa a la vez.
        /// </summary>
        Task<Election?> GetActiveElectionAsync();

        /// <summary>
        /// Verifica rápidamente si existe alguna elección con estado 'Activa'.
        /// </summary>
        Task<bool> AnyActiveElectionExistsAsync();

        /// <summary>
        /// Obtiene una lista de los años en los que existen elecciones registradas.
        /// </summary>
        Task<IEnumerable<int>> GetElectoralYearsAsync();

        /// <summary>
        /// Obtiene los nombres de los partidos políticos que tienen candidatos faltantes para los puestos activos.
        /// </summary>
        Task<IEnumerable<string>> GetPartiesWithMissingCandidatesAsync();

        /// <summary>
        /// Verifica si existe al menos un puesto electivo activo y al menos dos partidos activos.
        /// </summary>
        Task<(bool HasActivePositions, bool HasMinimumParties)> GetElectoralBasicsStatusAsync();
    }
}
