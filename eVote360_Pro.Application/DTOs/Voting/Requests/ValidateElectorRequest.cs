using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.DTOs.Voting.Requests
{
    /// <summary>
    /// Contrato de entrada para la validación inicial del elector (Cédula + OCR).
    /// </summary>
    public record ValidateElectorRequest(string IdentityDocument, IFormFile IdCardImage);
}
