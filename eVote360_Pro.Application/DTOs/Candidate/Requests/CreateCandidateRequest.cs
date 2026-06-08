using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.DTOs.Candidate.Requests
{
    /// <summary>
    /// Contrato de entrada para el registro de un nuevo candidato.
    /// </summary>
    public record CreateCandidateRequest(
        string FirstName,
        string LastName,
        IFormFile PhotoFile,
        int? OriginalPartyId // El service lo asignará automáticamente si es dirigente
    );
}
