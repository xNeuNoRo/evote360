namespace eVote360_Pro.Application.DTOs.PoliticalAlliance.Responses
{
    /// <summary>
    /// Contrato de salida con los detalles de una alianza o solicitud de pacto.
    /// </summary>
    public record AllianceResponse(
        int Id,
        int RequesterPartyId,
        string RequesterPartyName,
        int ReceiverPartyId,
        string ReceiverPartyName,
        string Status,
        DateTime CreatedAt,
        DateTime? AcceptedAt
    );
}
