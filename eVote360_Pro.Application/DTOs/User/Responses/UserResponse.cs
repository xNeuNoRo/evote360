namespace eVote360_Pro.Application.DTOs.User.Responses
{
    /// <summary>
    /// Contrato de salida con la información detallada del usuario.
    /// </summary>
    public record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string FullName,
        string Username,
        string Email,
        int RoleId,
        string RoleName,
        bool IsActive,
        DateTime CreatedAt,
        int? AssignedPartyId,
        string? AssignedPartyName
    );
}
