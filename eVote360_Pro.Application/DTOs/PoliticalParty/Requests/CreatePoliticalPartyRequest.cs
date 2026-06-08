using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.DTOs.PoliticalParty.Requests
{
    /// <summary>
    /// Contrato de entrada para la creación de un nuevo partido político.
    /// </summary>
    public record CreatePoliticalPartyRequest(
        string Name,
        string Acronym,
        string? Description,
        IFormFile LogoFile
    );
}
