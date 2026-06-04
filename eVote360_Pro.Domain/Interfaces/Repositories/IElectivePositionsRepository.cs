using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IElectivePositionsRepository : IGenericRepository<ElectivePositions>
    {
        /// <summary>
        /// Verifica si el puesto tiene candidatos activos asignados actualmente.
        /// </summary>
        Task<bool> HasActiveCandidatesAsync(int positionId);

        /// <summary>
        /// Verifica si el puesto ha sido utilizado en alguna configuración electoral (Pendiente, Activa o Finalizada).
        /// </summary>
        Task<bool> WasUsedInAnyElectionAsync(int positionId);
    }
}
