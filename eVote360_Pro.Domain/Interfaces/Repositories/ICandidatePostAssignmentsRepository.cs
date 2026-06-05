using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface ICandidatePostAssignmentsRepository
        : IGenericRepository<CandidatePostAssignment, int>
    {
        /// <summary>
        /// Verifica si un candidato ya tiene una asignación a cualquier puesto dentro de un partido específico.
        /// </summary>
        Task<bool> IsCandidateAssignedToAnyPostInPartyAsync(int candidateId, int partyId);

        /// <summary>
        /// Verifica si un puesto electivo ya está ocupado por algún candidato dentro de un partido específico.
        /// </summary>
        Task<bool> IsPositionOccupiedInPartyAsync(int positionId, int partyId);
    }
}
