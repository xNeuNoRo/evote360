using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de votos.
    /// Realiza conteos y agregaciones en tiempo real para los reportes de resultados.
    /// </summary>
    public class VoteRepository : GenericRepository<Vote, Guid>, IVoteRepository
    {
        public VoteRepository(AppDbContext context)
            : base(context) { }

        public async Task<int> GetTotalVotesByPositionAsync(Guid electionId, int positionId)
        {
            // Total de votos emitidos para un cargo, incluyendo la opción 'Ninguno'
            return await _dbSet.CountAsync(v =>
                v.ElectionId == electionId && v.PositionId == positionId
            );
        }

        public async Task<
            IEnumerable<(int? CandidateId, int? PartyId, int VoteCount)>
        > GetVotesDistributionAsync(Guid electionId, int positionId)
        {
            // Agrupamos los votos por Candidato/Partido y los cuenta
            // Esto devuelve automáticamente una fila con nulos para los votos 'Ninguno'
            var distribution = await _dbSet
                .Where(v => v.ElectionId == electionId && v.PositionId == positionId)
                .GroupBy(v => new { v.CandidateId, v.PartyId })
                .Select(g => new
                {
                    g.Key.CandidateId,
                    g.Key.PartyId,
                    Count = g.Count(),
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return distribution.Select(x => (x.CandidateId, x.PartyId, x.Count));
        }

        public async Task<bool> IsTieInFirstPlaceAsync(Guid electionId, int positionId)
        {
            // Obtenemos el conteo de votos de todas las opciones para ese puesto
            var results = await _dbSet
                .Where(v => v.ElectionId == electionId && v.PositionId == positionId)
                .GroupBy(v => new { v.CandidateId, v.PartyId })
                .Select(g => g.Count())
                .ToListAsync();

            // Si no hay votos, no hay empate
            if (!results.Any())
                return false;

            // Buscamos el valor máximo de votos
            int maxVotes = results.Max();

            // Si hay más de una opción que alcanzó el máximo, quiere decir q existe un empate
            return results.Count(v => v == maxVotes) > 1;
        }

        public async Task<int> GetTotalVoterParticipationAsync(Guid electionId)
        {
            // Cuenta ciudadanos únicos que participaron
            return await _context.VoterParticipations.CountAsync(p => p.ElectionId == electionId);
        }
    }
}
