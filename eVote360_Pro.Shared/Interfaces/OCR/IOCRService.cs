using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Shared.Interfaces.OCR
{
    /// <summary>
    /// Contrato para el procesamiento avanzado de reconocimiento óptico y validación de documentos.
    /// </summary>
    public interface IOCRService
    {
        /// <summary>
        /// Procesa una imagen de cédula, valida el formato del documento y extrae el número de identidad.
        /// </summary>
        /// <param name="idCardImage">Archivo de imagen subido por el ciudadano.</param>
        /// <returns>Un objeto OCRResponse con los datos extraídos y validaciones de confianza.</returns>
        Task<OCRResponse> ProcessIdentityCardAsync(IFormFile idCardImage);
    }
}
