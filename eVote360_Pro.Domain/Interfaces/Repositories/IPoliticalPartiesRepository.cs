using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IPoliticalPartiesRepository : IGenericRepository<PoliticalParties>
    {
        Task<bool> ExistsByAcronymAsync(string acronym, int? excludeId = null);
        Task<bool> HasActiveCandidatesAsync(int partyId);
        Task<bool> HasActiveLeaderAsync(int partyId);
    }
}
