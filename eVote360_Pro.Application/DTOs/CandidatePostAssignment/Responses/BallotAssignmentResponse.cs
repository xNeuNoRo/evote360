namespace eVote360_Pro.Application.DTOs.CandidatePostAssignment.Responses
{
    /// <summary>
    /// Contrato de salida con los detalles de una posición en la boleta electoral.
    /// </summary>
    public record BallotAssignmentResponse(
        int Id,
        int PositionId,
        string PositionName,
        int CandidateId,
        string CandidateName,
        string CandidatePhotoUrl,
        bool IsAlly,
        string CandidateOriginalPartyName
    );
}
