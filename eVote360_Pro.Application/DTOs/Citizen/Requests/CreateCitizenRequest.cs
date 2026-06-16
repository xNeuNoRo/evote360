namespace eVote360_Pro.Application.DTOs.Citizen.Requests
{
    /// <summary>
    /// Contrato de entrada para el registro de un nuevo ciudadano.
    /// </summary>
    public record CreateCitizenRequest(
        string IdentityDocument,
        string FirstName,
        string LastName,
        string Email,
        bool IsActive = true
    );
}
