using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de elecciones.
    /// Gestiona el ciclo de vida electoral y valida la integridad de los procesos.
    /// </summary>
    public class ElectionRepository : GenericRepository<Election, Guid>, IElectionRepository
    {
        public ElectionRepository(AppDbContext context)
            : base(context) { }

        public async Task<Election?> GetActiveElectionAsync()
        {
            // Solo puede existir una elección activa a la vez
            return await _dbSet.FirstOrDefaultAsync(e =>
                e.Status == ElectionStatus.Active && e.IsActive
            );
        }

        public async Task<bool> AnyActiveElectionExistsAsync()
        {
            return await _dbSet.AnyAsync(e => e.Status == ElectionStatus.Active && e.IsActive);
        }

        public async Task<IEnumerable<int>> GetElectoralYearsAsync()
        {
            // Obtiene los años únicos de las elecciones registradas para filtros del dashboard
            return await _dbSet
                .Where(e => e.IsActive)
                .Select(e => e.RealizationDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetPartiesWithMissingCandidatesAsync()
        {
            // Obtenemos los partidos activos q no tienen candidatos asignados a ningún cargo activo
            var activePositionCount = await _context.ElectivePositions.CountAsync(p => p.IsActive);

            // Si no hay cargos activos, quiere decir q no hay partidos con candidatos faltantes
            if (activePositionCount == 0)
                return Enumerable.Empty<string>();

            // Obtenemos el nombre de los partidos activos junto
            // con la cantidad de candidatos asignados a cargos activos
            var partiesStatus = await _context
                .PoliticalParties.Where(p => p.IsActive)
                .Select(p => new
                {
                    p.Name,
                    AssignedCount = _context.CandidatePostAssignments.Count(a =>
                        a.PartyId == p.Id && a.Position.IsActive
                    ),
                })
                .ToListAsync();

            // Devolvemos solo los nombres de los partidos
            // que tienen menos candidatos asignados que cargos activos
            return partiesStatus
                .Where(x => x.AssignedCount < activePositionCount)
                .Select(x => x.Name);
        }

        public async Task<(
            bool HasActivePositions,
            bool HasMinimumParties
        )> GetElectoralBasicsStatusAsync()
        {
            bool hasPositions = await _context.ElectivePositions.AnyAsync(p => p.IsActive);
            int partiesCount = await _context.PoliticalParties.CountAsync(p => p.IsActive);

            return (hasPositions, partiesCount >= 2);
        }
    }
}
