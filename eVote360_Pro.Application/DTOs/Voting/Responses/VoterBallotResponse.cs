namespace eVote360_Pro.Application.DTOs.Voting.Responses
{
    /// <summary>
    /// Representa la boleta electoral completa que se muestra al elector tras validar su OTP.
    /// </summary>
    public record VoterBallotResponse(
        Guid ElectionId,
        string ElectionName,
        List<BallotPositionResponse> Positions
    );

    public record BallotPositionResponse(
        int PositionId,
        string PositionName,
        string Description,
        List<BallotCandidateResponse> Options // Incluye candidatos propios y aliados
    );

    public record BallotCandidateResponse(
        int? CandidateId, // Nulo para la opción "Ninguno"
        string CandidateName,
        int? PartyId, // Nulo para la opción "Ninguno"
        string PartyName,
        string PartyAcronym,
        string PhotoUrl,
        string PartyLogoUrl
    );
}
