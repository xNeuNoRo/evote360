namespace eVote360_Pro.Application.DTOs.Citizen.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un ciudadano.
    /// </summary>
    public record UpdateCitizenRequest(
        int Id,
        string IdentityDocument,
        string FirstName,
        string LastName,
        string Email,
        bool IsActive
    );
}
