using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IElectivePositionsRepository : IGenericRepository<ElectivePosition, int>
    {
        /// <summary>
        /// Verifica si el puesto tiene candidatos activos asignados actualmente.
        /// </summary>
        Task<bool> HasActiveCandidatesAsync(int positionId);

        /// <summary>
        /// Verifica si el puesto ha sido utilizado en alguna configuración electoral.
        /// </summary>
        Task<bool> WasUsedInAnyElectionAsync(int positionId);

        /// <summary>
        /// Verifica si ya existe un puesto con el mismo nombre.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
