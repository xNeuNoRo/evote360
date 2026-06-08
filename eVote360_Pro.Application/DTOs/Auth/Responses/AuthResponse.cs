namespace eVote360_Pro.Application.DTOs.Auth.Responses
{
    /// <summary>
    /// Contrato de salida con la información del usuario y su token de acceso.
    /// </summary>
    public record AuthResponse(
        Guid UserId,
        string FullName,
        string Email,
        string RoleName,
        int? PartyId,
        string Token,
        DateTime Expiration
    );
}
