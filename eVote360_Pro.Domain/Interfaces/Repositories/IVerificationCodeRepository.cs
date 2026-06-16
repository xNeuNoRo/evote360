using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y validación de los códigos de verificación (OTP).
    /// Encapsula las reglas de seguridad y expiración del acceso del elector.
    /// </summary>
    public interface IVerificationCodeRepository : IGenericRepository<VerificationCode, Guid>
    {
        /// <summary>
        /// Obtiene un código de verificación que no ha sido usado y que pertenece a un ciudadano y elección específicos.
        /// </summary>
        Task<VerificationCode?> GetValidCodeAsync(int citizenId, Guid electionId, string code);

        /// <summary>
        /// Invalida (marca como usados) todos los códigos previos de un ciudadano para una elección específica.
        /// </summary>
        Task InvalidatePreviousCodesAsync(int citizenId, Guid electionId);

        /// <summary>
        /// Verifica si un ciudadano ha solicitado demasiados códigos en un corto periodo de tiempo.
        /// </summary>
        Task<int> CountRecentRequestsAsync(int citizenId, Guid electionId, DateTime since);
    }
}
