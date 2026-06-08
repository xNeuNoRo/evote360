namespace eVote360_Pro.Application.DTOs.Election.Responses
{
    /// <summary>
    /// Contrato de salida con la información detallada del proceso electoral y estado de activación.
    /// </summary>
    public record ElectionResponse(
        Guid Id,
        string Name,
        DateTime RealizationDate,
        string Status,
        int Year,
        bool IsActive,
        bool CanActivate,
        IEnumerable<string> MissingParties
    );
}
