namespace eVote360_Pro.Application.DTOs.PoliticalAlliance.Requests
{
    /// <summary>
    /// Contrato de entrada para solicitar una alianza con otro partido.
    /// </summary>
    public record CreateAllianceRequest(int ReceiverPartyId);
}
