using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IPoliticalPartiesRepository : IGenericRepository<PoliticalParties>
    {
        /// <summary>
        /// Verifica si el partido tiene candidatos activos registrados.
        /// </summary>
        Task<bool> HasActiveCandidatesAsync(int partyId);

        /// <summary>
        /// Verifica si el partido tiene un dirigente político activo asignado.
        /// </summary>
        Task<bool> HasActiveLeaderAsync(int partyId);

        /// <summary>
        /// Verifica si el partido ha participado en alguna elección previa o activa.
        /// </summary>
        Task<bool> WasUsedInAnyElectionAsync(int partyId);
    }
}
