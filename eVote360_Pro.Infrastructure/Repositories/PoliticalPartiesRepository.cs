using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class PoliticalPartiesRepository : GenericRepository<PoliticalParties>, IPoliticalPartiesRepository
    {
        public PoliticalPartiesRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByAcronymAsync(string acronym, int? excludeId = null)
        {
            string cleanAcronym = acronym.Trim().ToUpper(); 
            return await _context.PoliticalParties
                .AnyAsync(p => p.Acronym.ToUpper() == cleanAcronym && (!excludeId.HasValue || p.Id != excludeId));
        }

        public async Task<bool> HasActiveCandidatesAsync(int partyId)
        {
            // Requerimiento: No desactivar si tiene candidatos activos registrados 
            return await _context.Candidates
                .AnyAsync(c => c.OriginalPartyId == partyId && c.IsActive);
        }

        public async Task<bool> HasActiveLeaderAsync(int partyId)
        {
            // Requerimiento: No desactivar si tiene un dirigente político activo asignado 
            return await _context.PoliticalLeaderAssignments
                .AnyAsync(l => l.PartyId == partyId && l.IsActive);
        }
    }
}
