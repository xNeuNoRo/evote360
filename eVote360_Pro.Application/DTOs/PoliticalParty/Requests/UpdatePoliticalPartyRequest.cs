using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.DTOs.PoliticalParty.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un partido político.
    /// </summary>
    public record UpdatePoliticalPartyRequest(
        int Id,
        string Name,
        string Acronym,
        string? Description,
        IFormFile? LogoFile, // Opcional en edición
        bool IsActive
    );
}
