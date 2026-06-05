using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    public interface ICandidatesRepository : IGenericRepository<Candidate, int>
    {
        /// <summary>
        /// Verifica si el candidato está participando en una elección con estado 'Activa'.
        /// </summary>
        Task<bool> IsParticipatingInActiveElectionAsync(int candidateId);

        /// <summary>
        /// Verifica si el candidato ha participado en alguna elección previa o activa.
        /// </summary>
        Task<bool> HasParticipatedInAnyElectionAsync(int candidateId);
    }
}
