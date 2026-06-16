using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de asignación de dirigentes.
    /// Asegura la vinculación física 1:1 entre usuarios y partidos.
    /// </summary>
    public class PoliticalLeaderAssignmentRepository
        : GenericRepository<PoliticalLeaderAssignment, Guid>,
            IPoliticalLeaderAssignmentsRepository
    {
        public PoliticalLeaderAssignmentRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> IsUserAlreadyLeaderAsync(Guid userId)
        {
            return await _dbSet.AnyAsync(a => a.Id == userId);
        }

        public async Task<bool> HasPartyAlreadyLeaderAsync(int partyId)
        {
            return await _dbSet.AnyAsync(a => a.PartyId == partyId);
        }
    }
}
