namespace eVote360_Pro.Application.DTOs.Auth.Responses
{
    /// <summary>
    /// Contrato de salida con la información del usuario autenticado para la sesión.
    /// </summary>
    public record AuthResponse(
        Guid UserId,
        string FullName,
        string Email,
        string RoleName,
        int? PartyId
    );
}
