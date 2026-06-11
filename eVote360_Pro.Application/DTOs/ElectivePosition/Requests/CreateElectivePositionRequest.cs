namespace eVote360_Pro.Application.DTOs.ElectivePosition.Requests
{
    /// <summary>
    /// Contrato de entrada para la creación de un nuevo puesto electivo.
    /// </summary>
    public record CreateElectivePositionRequest(string Name, string Description);
}
