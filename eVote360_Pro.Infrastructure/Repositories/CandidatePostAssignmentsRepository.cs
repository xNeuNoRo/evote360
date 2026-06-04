using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    public class CandidatePostAssignmentsRepository : GenericRepository<CandidatePostAssignments>, ICandidatePostAssignmentsRepository
    {
        public CandidatePostAssignmentsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<CandidatePostAssignments?> GetAssignmentAsync(int partyId, int positionId)
        {
            return await _context.CandidatePostAssignments
                .FirstOrDefaultAsync(a => a.PartyId == partyId && a.PositionId == positionId);
        }

        public async Task<bool> IsCandidateAssignedAsync(int candidateId, int partyId)
        {
            // Requerimiento: Un candidato no puede aspirar a más de un puesto dentro del mismo partido
            return await _context.CandidatePostAssignments
                .AnyAsync(a => a.CandidateId == candidateId && a.PartyId == partyId);
        }

        public async Task<bool> IsPositionOccupiedAsync(int positionId, int partyId)
        {
            // Requerimiento: El puesto no debe tener ya otro candidato asignado dentro del partido
            return await _context.CandidatePostAssignments
                .AnyAsync(a => a.PositionId == positionId && a.PartyId == partyId);
        }
    }
}
