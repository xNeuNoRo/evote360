using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de asignaciones de candidatos.
    /// Define la composición de la boleta electoral por partido.
    /// </summary>
    public class CandidatePostAssignmentRepository
        : GenericRepository<CandidatePostAssignment, int>,
            ICandidatePostAssignmentsRepository
    {
        public CandidatePostAssignmentRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> IsCandidateAssignedToAnyPostInPartyAsync(
            int candidateId,
            int partyId
        )
        {
            return await _dbSet.AnyAsync(a => a.CandidateId == candidateId && a.PartyId == partyId);
        }

        public async Task<bool> IsPositionOccupiedInPartyAsync(int positionId, int partyId)
        {
            return await _dbSet.AnyAsync(a => a.PositionId == positionId && a.PartyId == partyId);
        }

        public async Task<CandidatePostAssignment?> GetOriginalAssignmentAsync(int candidateId)
        {
            return await _context
                .CandidatePostAssignments.Include(a => a.Position)
                .FirstOrDefaultAsync(a =>
                    a.CandidateId == candidateId && a.PartyId == a.Candidate.OriginalPartyId
                );
        }
    }
}
