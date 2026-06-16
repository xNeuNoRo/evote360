using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IPoliticalPartiesRepository : IGenericRepository<PoliticalParty, int>
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

        /// <summary>
        /// Verifica si ya existe un partido con las mismas siglas.
        /// </summary>
        Task<bool> ExistsByAcronymAsync(string acronym, int? excludeId = null);

        /// <summary>
        /// Verifica si ya existe un partido con el mismo nombre.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
