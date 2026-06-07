using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de participación electoral.
    /// </summary>
    public class VoterParticipationRepository
        : GenericRepository<VoterParticipation, int>,
            IVoterParticipationRepository
    {
        public VoterParticipationRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> HasAlreadyVotedAsync(int citizenId, Guid electionId)
        {
            return await _dbSet.AnyAsync(p =>
                p.CitizenId == citizenId && p.ElectionId == electionId
            );
        }

        public async Task<int> GetTotalVotersByElectionAsync(Guid electionId)
        {
            return await _dbSet.CountAsync(p => p.ElectionId == electionId);
        }
    }
}
