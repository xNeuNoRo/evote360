namespace eVote360_Pro.Application.DTOs.Voting.Responses
{
    /// <summary>
    /// Contrato de salida para el reporte final de resultados de una elección.
    /// </summary>
    public record ResultReportResponse(
        Guid ElectionId,
        string ElectionName,
        int TotalVoters,
        List<PositionResultResponse> ResultsByPosition
    );

    public record PositionResultResponse(
        int PositionId,
        string PositionName,
        bool IsTie,
        List<CandidateResultResponse> Candidates
    );

    public record CandidateResultResponse(
        int? CandidateId,
        string CandidateName,
        int? PartyId,
        string PartyName,
        string PhotoUrl,
        int VotesCount,
        double Percentage
    );
}
