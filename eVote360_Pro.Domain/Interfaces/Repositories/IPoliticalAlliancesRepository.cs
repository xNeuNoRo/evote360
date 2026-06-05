using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IPoliticalAlliancesRepository : IGenericRepository<PoliticalAlliance, int>
    {
        /// <summary>
        /// Verifica si existen asignaciones de candidatos aliados activas entre dos partidos.
        /// </summary>
        Task<bool> HasActiveAlliedAssignmentsAsync(int partyAId, int partyBId);

        /// <summary>
        /// Verifica si ya existe una solicitud de alianza o una alianza vigente entre dos partidos.
        /// </summary>
        Task<bool> AllianceOrRequestExistsAsync(int partyAId, int partyBId);
    }
}
