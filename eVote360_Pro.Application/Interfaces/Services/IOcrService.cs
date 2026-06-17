using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Contrato para el procesamiento avanzado de reconocimiento óptico y validación de documentos.
    /// </summary>
    public interface IOcrService
    {
        Task<OcrResponse> ProcessIdentityCardAsync(IFormFile idCardImage);
    }
}
