using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y consultas específicas de los ciudadanos registrados.
    /// Encapsula las reglas de validación electoral y mantenimiento.
    /// </summary>
    public interface ICitizenRepository : IGenericRepository<Citizen, int>
    {
        /// <summary>
        /// Obtiene un ciudadano por su número de documento de identidad (Cédula).
        /// </summary>
        Task<Citizen?> GetByIdentityDocumentAsync(string identityDocument);

        /// <summary>
        /// Verifica si un ciudadano ya ha participado en una elección específica.
        /// </summary>
        Task<bool> HasParticipatedInElectionAsync(int citizenId, Guid electionId);

        /// <summary>
        /// Verifica si el ciudadano ha participado en alguna elección previa o activa.
        /// </summary>
        Task<bool> HasParticipatedInAnyElectionAsync(int citizenId);
    }
}
