using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IPoliticalLeaderAssignmentsRepository
        : IGenericRepository<PoliticalLeaderAssignments>
    {
        /// <summary>
        /// Verifica si un usuario ya es dirigente de cualquier partido político.
        /// </summary>
        Task<bool> IsUserAlreadyLeaderAsync(int userId);

        /// <summary>
        /// Verifica si un partido político ya tiene un dirigente asignado.
        /// </summary>
        Task<bool> HasPartyAlreadyLeaderAsync(int partyId);
    }
}
