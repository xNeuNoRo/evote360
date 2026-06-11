namespace eVote360_Pro.Application.DTOs.User.Requests
{
    /// <summary>
    /// Contrato de entrada para la creación de un nuevo usuario.
    /// </summary>
    public record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        string Username,
        string Password,
        int RoleId
    );
}
