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
            // Obtenemos los IDs de los puestos electivos activos
            var activePositionIds = await _context
                .ElectivePositions.Where(p => p.IsActive)
                .Select(p => p.Id)
                .ToListAsync();

            // Si no hay puestos activos, no hay nada por reportar en vd
            if (!activePositionIds.Any())
                return Enumerable.Empty<string>();

            // Obtenemos los partidos activos
            var activeParties = await _context
                .PoliticalParties.Where(p => p.IsActive)
                .ToListAsync();

            // Creamos una lista para almacenar los nombres de los partidos que tienen vacios en sus candidaturas
            var partiesWithGaps = new List<string>();

            // Iteramos en cada partido activo
            foreach (var party in activeParties)
            {
                // Verificamos si tiene asignaciones para todos los puestos activos
                var assignmentsCount = await _context.CandidatePostAssignments.CountAsync(a =>
                    a.PartyId == party.Id && activePositionIds.Contains(a.PositionId)
                );

                // Si el número de asignaciones es menor que el número de puestos activos,
                // quiere decir q hay un vacío en la boleta de ese partido
                if (assignmentsCount < activePositionIds.Count)
                {
                    partiesWithGaps.Add(party.Name);
                }
            }

            return partiesWithGaps;
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
