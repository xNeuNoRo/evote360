namespace eVote360_Pro.Application.DTOs.Candidate.Responses
{
    /// <summary>
    /// Contrato de salida con la información del candidato y su partido de origen.
    /// </summary>
    public record CandidateResponse(
        int Id,
        string FullName,
        string PhotoUrl,
        int OriginalPartyId,
        string OriginalPartyName,
        bool IsActive,
        bool IsImmutable
    );
}
