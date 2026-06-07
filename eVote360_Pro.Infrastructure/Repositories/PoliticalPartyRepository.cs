using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de partidos políticos.
    /// Gestiona las reglas de unicidad e integridad referencial de los partidos.
    /// </summary>
    public class PoliticalPartyRepository
        : GenericRepository<PoliticalParty, int>,
            IPoliticalPartiesRepository
    {
        public PoliticalPartyRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> HasActiveCandidatesAsync(int partyId)
        {
            return await _context.Candidates.AnyAsync(c =>
                c.OriginalPartyId == partyId && c.IsActive
            );
        }

        public async Task<bool> HasActiveLeaderAsync(int partyId)
        {
            return await _context.PoliticalLeaderAssignments.AnyAsync(l =>
                l.PartyId == partyId && l.IsActive
            );
        }

        public async Task<bool> WasUsedInAnyElectionAsync(int partyId)
        {
            return await _context.Votes.AnyAsync(v => v.PartyId == partyId)
                || await _context.CandidatePostAssignments.AnyAsync(a => a.PartyId == partyId);
        }

        public async Task<bool> ExistsByAcronymAsync(string acronym, int? excludeId = null)
        {
            string upperAcronym = acronym.ToUpperInvariant().Trim();
            return await _dbSet.AnyAsync(p => p.Acronym == upperAcronym && p.Id != excludeId);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            string normalizedName = name.ToLowerInvariant().Trim();
            return await _dbSet.AnyAsync(p =>
                p.Name.ToLower() == normalizedName && p.Id != excludeId
            );
        }
    }
}
