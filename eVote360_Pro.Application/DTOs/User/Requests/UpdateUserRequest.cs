namespace eVote360_Pro.Application.DTOs.User.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un usuario existente.
    /// </summary>
    public record UpdateUserRequest(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Username,
        string? Password, // Opcional en edición
        int RoleId,
        bool IsActive
    );
}
