using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de candidatos.
    /// Resuelve consultas específicas sobre la participación y elegibilidad de los candidatos.
    /// </summary>
    public class CandidateRepository : GenericRepository<Candidate, int>, ICandidatesRepository
    {
        public CandidateRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> IsParticipatingInActiveElectionAsync(int candidateId)
        {
            return await _context.CandidatePostAssignments.AnyAsync(a =>
                a.CandidateId == candidateId
                && _context.Elections.Any(e => e.Status == ElectionStatus.Active)
            );
        }

        public async Task<bool> HasParticipatedInAnyElectionAsync(int candidateId)
        {
            return await _context.Votes.AnyAsync(v => v.CandidateId == candidateId);
        }

        public async Task<bool> IsAssignedToAnyPostAsync(int candidateId)
        {
            return await _context.CandidatePostAssignments.AnyAsync(a =>
                a.CandidateId == candidateId
            );
        }
    }
}
