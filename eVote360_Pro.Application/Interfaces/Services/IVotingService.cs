using eVote360_Pro.Application.DTOs.Voting.Requests;
using eVote360_Pro.Application.DTOs.Voting.Responses;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Orquesta el flujo del elector desde la identificación hasta el registro de su voto
    /// </summary>
    public interface IVotingService
    {
        /// <summary>
        /// Valida al ciudadano contra el padrón y realiza el OCR de su cédula.
        /// Si es exitoso, envía el código OTP por correo.
        /// </summary>
        Task<bool> ValidateAndSendOtpAsync(ValidateElectorRequest request);

        /// <summary>
        /// Verifica que el código ingresado coincida y no haya expirado.
        /// </summary>
        Task<VerifyCodeResponse> VerifyOtpAsync(VerifyCodeRequest request);

        /// <summary>
        /// Retorna la estructura de la boleta para que el ciudadano vote.
        /// </summary>
        Task<VoterBallotResponse> GetBallotAsync(Guid electionId);

        /// <summary>
        /// Registra la intención de voto de forma atómica y anónima.
        /// Garantiza la regla de voto único.
        /// </summary>
        Task SubmitVoteAsync(SubmitVoteRequest request);
    }
}
