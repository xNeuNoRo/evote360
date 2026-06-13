using System.Text.RegularExpressions;
using eVote360_Pro.Shared.Interfaces.OCR;
using eVote360_Pro.Shared.Interfaces.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Tesseract;

namespace eVote360_Pro.Infrastructure.OCR
{
    /// <summary>
    /// Servicio orquestador de OCR que utiliza el motor de visión abstracto para procesar identidades.
    /// </summary>
    public class OcrService : IOcrService
    {
        private readonly IFileService _fileService;
        private readonly ILogger<OcrService> _logger;
        private readonly string _tessdataPath;

        public OcrService(IFileService fileService, ILogger<OcrService> logger)
        {
            _fileService = fileService;
            _logger = logger;
            _tessdataPath = Path.Combine(AppContext.BaseDirectory, "tessdata");
        }

        public async Task<OcrResponse> ProcessIdentityCardAsync(IFormFile idCardImage)
        {
            var response = new OcrResponse();
            string tempAbsPath = Path.GetTempFileName();
            string optAbsPath = tempAbsPath + "_opt.png";

            try
            {
                // Guardamos la imagen temporalmente en la ruta "/tmp" del SO para procesarla con OpenCV
                using (var stream = new FileStream(tempAbsPath, FileMode.Create))
                {
                    await idCardImage.CopyToAsync(stream);
                }

                using (var src = new Mat(tempAbsPath))
                {
                    response.IsDocumentValid = VisionProcessor.ContainsDocument(src);

                    if (!response.IsDocumentValid)
                    {
                        _logger.LogWarning(
                            "La imagen procesada no tiene una forma geometrica de documento valida."
                        );
                        response.IsSuccess = false;
                        response.ErrorMessage =
                            "La imagen proporcionada no parece ser un documento de identidad válido. Asegúrese de que los bordes del documento sean visibles y haya buena iluminación.";
                        return response;
                    }

                    // Preparamos la imagen para el OCR
                    using (var optimized = VisionProcessor.PrepareForTextExtraction(src))
                    {
                        optimized.SaveImage(optAbsPath);
                    }
                }

                // Ejecutamos el OCR utilizando Tesseract en la imagen optimizada
                using (var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default))
                // Obtenemos el resultado del OCR y extraemos el numero de identidad utilizando un patrón regex específico
                using (var img = Pix.LoadFromFile(optAbsPath))
                using (var page = engine.Process(img))
                {
                    response.Confidence = page.GetMeanConfidence();
                    response.IdentityNumber = ExtractCedulaNumber(page.GetText());
                }

                // Evaluamos el resultado del OCR para determinar si fue exitoso o no
                response.IsSuccess = !string.IsNullOrEmpty(response.IdentityNumber);
                if (!response.IsSuccess)
                    response.ErrorMessage =
                        "No se pudo extraer el número de identidad. Intente con otra fotografía más clara.";

                return response;
            }
            catch (Exception ex)
            {
                // Logueamos el error con detalle para facilitar la investigación y solución de problemas
                _logger.LogError(ex, "Fallo crítico en el proceso de OCR.");
                response.IsSuccess = false;

                // Si la excepción es por falta de dependencias nativas,
                // damos un mensaje específico para facilitar el diagnóstico del problema en el servidor
                if (ex.InnerException is DllNotFoundException)
                {
                    response.ErrorMessage =
                        "Error interno: Faltan dependencias nativas del motor OCR en el servidor.";
                }
                else
                {
                    response.ErrorMessage = $"Error en el motor de detección: {ex.Message}";
                }
                return response;
            }
            finally
            {
                // Limpiamos los archivos temporales para evitar acumularlos
                if (File.Exists(tempAbsPath))
                    File.Delete(tempAbsPath);
                if (File.Exists(optAbsPath))
                    File.Delete(optAbsPath);
            }
        }

        /// <summary>
        /// Extrae el número de cédula de identidad del texto OCR
        /// utilizando una expresión regular específica para el formato dominicano.
        /// </summary>
        private static string? ExtractCedulaNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var match = Regex.Match(text, @"\b\d{3}[-\s]?\d{7}[-\s]?\d{1}\b");
            return match.Success ? match.Value.Replace("-", "").Replace(" ", "") : null;
        }
    }
}
