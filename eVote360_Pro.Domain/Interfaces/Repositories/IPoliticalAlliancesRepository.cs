using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IPoliticalAlliancesRepository : IGenericRepository<PoliticalAlliances>
    {
        Task<IEnumerable<PoliticalAlliances>> GetActiveAlliancesByPartyAsync(int partyId);
        Task<bool> AllianceExistsAsync(int partyAId, int partyBId);
    }
}
