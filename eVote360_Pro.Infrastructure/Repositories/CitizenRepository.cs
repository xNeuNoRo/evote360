using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de ciudadanos.
    /// Encapsula las reglas de validación electoral y mantenimiento de electores.
    /// </summary>
    public class CitizenRepository : GenericRepository<Citizen, int>, ICitizenRepository
    {
        public CitizenRepository(AppDbContext context)
            : base(context) { }

        public async Task<Citizen?> GetByIdentityDocumentAsync(string identityDocument)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.IdentityDocument == identityDocument);
        }

        public async Task<bool> HasParticipatedInElectionAsync(int citizenId, Guid electionId)
        {
            return await _context.VoterParticipations.AnyAsync(p =>
                p.CitizenId == citizenId && p.ElectionId == electionId
            );
        }

        public async Task<bool> HasParticipatedInAnyElectionAsync(int citizenId)
        {
            return await _context.VoterParticipations.AnyAsync(p => p.CitizenId == citizenId);
        }
    }
}
