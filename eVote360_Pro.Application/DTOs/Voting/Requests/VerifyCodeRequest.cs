namespace eVote360_Pro.Application.DTOs.Voting.Requests
{
    /// <summary>
    /// Contrato de entrada para validar el código OTP recibido por correo.
    /// </summary>
    public record VerifyCodeRequest(int CitizenId, Guid ElectionId, string Code);
}
