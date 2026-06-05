using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y agregación de votos.
    /// Encapsula la lógica de conteo, porcentajes y detección de empates en tiempo real.
    /// </summary>
    public interface IVoteRepository : IGenericRepository<Vote, Guid>
    {
        /// <summary>
        /// Obtiene el conteo total de votos para una elección y un puesto electivo específico.
        /// </summary>
        Task<int> GetTotalVotesByPositionAsync(Guid electionId, int positionId);

        /// <summary>
        /// Obtiene el conteo de votos agrupado por opción (Candidato + Partido).
        /// Incluye la opción 'Ninguno' (CandidatoId y PartyId nulos).
        /// </summary>
        /// <returns>Una colección de tuplas con el ID del candidato, el ID del partido y la cantidad de votos.</returns>
        Task<
            IEnumerable<(int? CandidateId, int? PartyId, int VoteCount)>
        > GetVotesDistributionAsync(Guid electionId, int positionId);

        /// <summary>
        /// Verifica si existe un empate en el primer lugar para un puesto específico en una elección.
        /// </summary>
        Task<bool> IsTieInFirstPlaceAsync(Guid electionId, int positionId);

        /// <summary>
        /// Obtiene el total de ciudadanos que han participado en una elección específica.
        /// </summary>
        Task<int> GetTotalVoterParticipationAsync(Guid electionId);
    }
}
