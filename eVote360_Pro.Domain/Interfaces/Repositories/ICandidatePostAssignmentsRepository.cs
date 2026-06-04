using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface ICandidatePostAssignmentsRepository : IGenericRepository<CandidatePostAssignments>
    {
        Task<CandidatePostAssignments?> GetAssignmentAsync(int partyId, int positionId);
        Task<bool> IsCandidateAssignedAsync(int candidateId, int partyId);
        Task<bool> IsPositionOccupiedAsync(int positionId, int partyId);
    }
}
