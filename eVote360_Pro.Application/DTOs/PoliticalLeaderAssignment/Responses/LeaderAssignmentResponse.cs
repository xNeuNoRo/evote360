namespace eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Responses
{
    /// <summary>
    /// Contrato de salida para el listado de asignaciones de dirigentes.
    /// </summary>
    public record LeaderAssignmentResponse(
        Guid UserId,
        string UserName,
        string UserFullName,
        int PartyId,
        string PartyName,
        string PartyAcronym,
        DateTime CreatedAt
    );
}
