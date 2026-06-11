namespace eVote360_Pro.Application.DTOs.Citizen.Responses
{
    /// <summary>
    /// Contrato de salida con la información del ciudadano y estados de inmutabilidad.
    /// </summary>
    public record CitizenResponse(
        int Id,
        string IdentityDocument,
        string FullName,
        string Email,
        bool IsActive,
        bool HasVotedInActiveElection,
        bool IsIdentityDocumentImmutable
    );
}
