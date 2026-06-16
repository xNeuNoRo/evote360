namespace eVote360_Pro.Application.DTOs.CandidatePostAssignment.Requests
{
    /// <summary>
    /// Contrato de entrada para asignar un candidato a un puesto en la boleta del partido.
    /// </summary>
    public record SaveBallotAssignmentRequest(int PositionId, int CandidateId, bool IsAlly);
}
