using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface IElectivePositionsRepository : IGenericRepository<ElectivePositions>
    {
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> HasActiveCandidatesAsync(int positionId);
    }
}
