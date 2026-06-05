using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Shared.Interfaces.OCR
{
    /// <summary>
    /// Contrato para el procesamiento de reconocimiento óptico de caracteres.
    /// </summary>
    public interface IOCRService
    {
        /// <summary>
        /// Procesa una imagen de cédula y extrae el número de documento.
        /// </summary>
        Task<string?> ExtractIdentityNumberAsync(IFormFile idCardImage);
    }
}
