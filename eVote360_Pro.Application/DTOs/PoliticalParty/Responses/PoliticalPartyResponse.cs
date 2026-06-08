namespace eVote360_Pro.Application.DTOs.PoliticalParty.Responses
{
    /// <summary>
    /// Contrato de salida con la información detallada del partido político.
    /// </summary>
    public record PoliticalPartyResponse(
        int Id,
        string Name,
        string Acronym,
        string? Description,
        string LogoUrl,
        bool IsActive,
        bool IsImmutable,
        bool HasLeaderAssigned
    );
}
