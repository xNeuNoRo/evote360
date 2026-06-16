namespace eVote360_Pro.Application.DTOs.Election.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de una elección existente.
    /// </summary>
    public record UpdateElectionRequest(Guid Id, string Name, DateTime RealizationDate);
}
