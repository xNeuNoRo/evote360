namespace eVote360_Pro.Application.DTOs.Auth.Requests
{
    /// <summary>
    /// Contrato de entrada para el inicio de sesión.
    /// </summary>
    public record LoginRequest(string Username, string Password);
}
