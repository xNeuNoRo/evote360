namespace eVote360_Pro.Application.DTOs.ElectivePosition.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un puesto electivo.
    /// </summary>
    public record UpdateElectivePositionRequest(
        int Id,
        string Name,
        string Description,
        bool IsActive
    );
}
