using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de puestos electivos.
    /// Gestiona las reglas de inmutabilidad y disponibilidad de los cargos electorales.
    /// </summary>
    public class ElectivePositionRepository
        : GenericRepository<ElectivePosition, int>,
            IElectivePositionsRepository
    {
        public ElectivePositionRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> HasActiveCandidatesAsync(int positionId)
        {
            return await _context.CandidatePostAssignments.AnyAsync(a =>
                a.PositionId == positionId && a.Candidate.IsActive
            );
        }

        public async Task<bool> WasUsedInAnyElectionAsync(int positionId)
        {
            return await _context.Votes.AnyAsync(v => v.PositionId == positionId)
                || await _context.CandidatePostAssignments.AnyAsync(a =>
                    a.PositionId == positionId
                );
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
