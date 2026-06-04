using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class PoliticalLeaderAssignmentsRepository : GenericRepository<PoliticalLeaderAssignments>, IPoliticalLeaderAssignmentsRepository
    {
        public PoliticalLeaderAssignmentsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsUserAlreadyLeaderAsync(int userId)
        {
            // Requerimiento: Un usuario solo puede ser dirigente de un único partido político a la vez
            return await _context.PoliticalLeaderAssignments
                .AnyAsync(l => l.UserId == userId);
        }

        public async Task<bool> HasPartyAlreadyLeaderAsync(int partyId)
        {
            // Requerimiento: Un partido político solo puede tener un único dirigente asignado
            return await _context.PoliticalLeaderAssignments
                .AnyAsync(l => l.PartyId == partyId);
        }

        public async Task<PoliticalLeaderAssignments?> GetActiveAssignmentByUserIdAsync(int userId)
        {
            // Requerimiento funcional: Permite mapear qué partido gestiona el dirigente autenticado
            return await _context.PoliticalLeaderAssignments
                .Include(l => l.Party) 
                .FirstOrDefaultAsync(l => l.UserId == userId);
        }
    }
}
