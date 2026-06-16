using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y consulta del registro de participación ciudadana.
    /// Garantiza físicamente el cumplimiento de la regla de un solo voto por elección.
    /// </summary>
    public interface IVoterParticipationRepository : IGenericRepository<VoterParticipation, int>
    {
        /// <summary>
        /// Verifica si un ciudadano ya ha registrado su participación en una elección específica.
        /// </summary>
        Task<bool> HasAlreadyVotedAsync(int citizenId, Guid electionId);

        /// <summary>
        /// Obtiene el total de ciudadanos que han ejercido su voto en una elección específica.
        /// </summary>
        Task<int> GetTotalVotersByElectionAsync(Guid electionId);
    }
}
