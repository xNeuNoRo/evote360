using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class CandidatesRepository : GenericRepository<Candidates>, ICandidatesRepository
    {
        public CandidatesRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Candidates>> GetByPartyIdAsync(int partyId)
        {
            // Requerimiento: El dirigente solo gestiona candidatos de su propio partido
            return await _context.Candidates
                .Where(c => c.OriginalPartyId == partyId)
                .ToListAsync();
        }

        public async Task<bool> IsAssignedToActivePostAsync(int candidateId)
        {
            // Requerimiento: No permitir inactivar si está asignado a un puesto vigente
            return await _context.CandidatePostAssignments
                .AnyAsync(a => a.CandidateId == candidateId);
        }
    }
}
