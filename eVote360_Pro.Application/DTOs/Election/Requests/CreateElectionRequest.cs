namespace eVote360_Pro.Application.DTOs.Election.Requests
{
    /// <summary>
    /// Contrato de entrada para la creación de un nuevo proceso electoral.
    /// </summary>
    public record CreateElectionRequest(string Name, DateTime RealizationDate);
}
