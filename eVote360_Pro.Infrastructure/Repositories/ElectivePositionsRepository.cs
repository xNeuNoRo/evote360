using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class ElectivePositionsRepository : GenericRepository<ElectivePositions>, IElectivePositionsRepository
    {
        public ElectivePositionsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            string cleanName = name.Trim().ToLower(); 
            return await _context.ElectivePositions
                .AnyAsync(p => p.Name.Trim().ToLower() == cleanName && (!excludeId.HasValue || p.Id != excludeId));
        }

        public async Task<bool> HasActiveCandidatesAsync(int positionId)
        {
            // Requerimiento: Validar si tiene candidatos asignados antes de inactivar 
            return await _context.CandidatePostAssignments
                .AnyAsync(a => a.PositionId == positionId); 
        }
    }
}
