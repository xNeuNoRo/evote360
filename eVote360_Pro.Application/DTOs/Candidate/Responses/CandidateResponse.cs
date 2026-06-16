namespace eVote360_Pro.Application.DTOs.Candidate.Responses
{
    /// <summary>
    /// Contrato de salida con la información del candidato y su partido de origen.
    /// </summary>
    public record CandidateResponse(
        int Id,
        string FirstName,
        string LastName,
        string FullName,
        string PhotoPath,
        int OriginalPartyId,
        string OriginalPartyName,
        string OriginalPartyAcronym,
        string OriginalPartyLogoPath,
        bool IsActive,
        int VotesCount
    );
}
