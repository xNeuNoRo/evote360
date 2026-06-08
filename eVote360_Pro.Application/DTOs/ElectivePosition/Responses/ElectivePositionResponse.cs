namespace eVote360_Pro.Application.DTOs.ElectivePosition.Responses
{
    /// <summary>
    /// Contrato de salida con la información del puesto electivo.
    /// </summary>
    public record ElectivePositionResponse(
        int Id,
        string Name,
        string Description,
        bool IsActive,
        bool IsImmutable
    );
}
