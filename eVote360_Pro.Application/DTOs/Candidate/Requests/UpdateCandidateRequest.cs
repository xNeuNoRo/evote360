using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.DTOs.Candidate.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un candidato.
    /// </summary>
    public record UpdateCandidateRequest(
        int Id,
        string FirstName,
        string LastName,
        IFormFile? PhotoFile, // Opcional en edición
        bool IsActive
    );
}
