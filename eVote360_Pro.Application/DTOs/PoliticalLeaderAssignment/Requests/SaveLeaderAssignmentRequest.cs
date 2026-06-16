namespace eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Requests
{
    /// <summary>
    /// Contrato para vincular a un usuario con rol Dirigente a un partido político.
    /// (Pág. 74: Paso final de la gestión de mandos).
    /// </summary>
    public record SaveLeaderAssignmentRequest(Guid UserId, int PartyId);
}
