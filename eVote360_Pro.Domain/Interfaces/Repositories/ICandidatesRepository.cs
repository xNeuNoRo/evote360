using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface ICandidatesRepository : IGenericRepository<Candidates>
    {
        Task<IEnumerable<Candidates>> GetByPartyIdAsync(int partyId);
        Task<bool> IsAssignedToActivePostAsync(int candidateId);
    }
}
