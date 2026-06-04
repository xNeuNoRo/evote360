using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class PoliticalAlliancesRepository : GenericRepository<PoliticalAlliances>, IPoliticalAlliancesRepository
    {
        public PoliticalAlliancesRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PoliticalAlliances>> GetActiveAlliancesByPartyAsync(int partyId)
        {
            // Requerimiento: Alianzas en estado 'Accepted' donde participe el partido
            return await _context.PoliticalAlliances
                .Where(a => (a.RequesterPartyId == partyId || a.ReceiverPartyId == partyId) 
                            && a.Status == AllianceStatus.Accepted)
                .ToListAsync();
        }

        public async Task<bool> AllianceExistsAsync(int partyAId, int partyBId)
        {
            // Requerimiento: Validar si existe una alianza vigente o solicitud pendiente en cualquier dirección
            return await _context.PoliticalAlliances
                .AnyAsync(a => ((a.RequesterPartyId == partyAId && a.ReceiverPartyId == partyBId) ||
                                (a.RequesterPartyId == partyBId && a.ReceiverPartyId == partyAId))
                               && a.Status != AllianceStatus.Rejected);
        }
    }
}
