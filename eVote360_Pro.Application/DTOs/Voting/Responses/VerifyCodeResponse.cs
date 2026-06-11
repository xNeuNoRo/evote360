namespace eVote360_Pro.Application.DTOs.Voting.Responses
{
    /// <summary>
    /// Contrato de salida tras validar el código OTP.
    /// </summary>
    public record VerifyCodeResponse(bool IsValid, string? ErrorMessage);
}
