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
            string? tempPath = null;
            string? optimizedPath = null;

            try
            {
                // Subimos la imagen a una ruta temporal para procesamiento
                tempPath = await _fileService.UploadTempFileAsync(idCardImage);

                // Obtenemos la ruta absoluta del archivo temporal
                string tempAbsPath = _fileService.GetAbsolutePath(tempPath);

                // Validamos que la imagen contenga un documento con forma válida antes de intentar OCR
                using (var src = new Mat(tempAbsPath))
                {
                    // Detectamos si la imagen tiene un documento con forma válida (cuadrilátero dominante)
                    response.IsDocumentValid = VisionProcessor.ContainsDocument(src);

                    if (!response.IsDocumentValid)
                        _logger.LogWarning(
                            "La imagen procesada no tiene una forma geometrica de documento valida."
                        );

                    // Preparamos la imagen para el OCR
                    using (var optimized = VisionProcessor.PrepareForTextExtraction(src))
                    {
                        optimizedPath = tempPath + "_opt.png";
                        string optAbsPath = _fileService.GetAbsolutePath(optimizedPath);
                        optimized.SaveImage(optAbsPath);
                    }
                }

                // Obtenemos la ruta absoluta del archivo optimizado para el OCR
                string finalAbsPath = _fileService.GetAbsolutePath(optimizedPath);

                // Ejecutamos el OCR utilizando Tesseract en la imagen optimizada
                using (var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default))
                // Cargamos la imagen optimizada y procesamos el texto
                using (var img = Pix.LoadFromFile(finalAbsPath))
                // Obtenemos el resultado del OCR y extraemos el numero de identidad utilizando un patrón regex específico
                using (var page = engine.Process(img))
                {
                    response.Confidence = page.GetMeanConfidence();
                    response.IdentityNumber = ExtractCedulaNumber(page.GetText());
                }

                // Evaluamos el resultado del OCR para determinar si fue exitoso o no
                response.IsSuccess = !string.IsNullOrEmpty(response.IdentityNumber);
                if (!response.IsSuccess)
                    response.ErrorMessage =
                        "No se pudo detectar un número de identidad en la imagen.";

                return response;
            }
            catch (Exception ex)
            {
                // Logueamos el error con detalle para facilitar la investigación y solución de problemas
                _logger.LogError(ex, "Fallo crítico en el proceso de OCR.");
                response.IsSuccess = false;
                response.ErrorMessage = $"Error en el motor de detección: {ex.Message}";
                return response;
            }
            finally
            {
                // Limpiamos los archivos temporales para evitar acumularlos
                if (tempPath != null)
                    _fileService.DeleteFile(tempPath);
                if (optimizedPath != null)
                    _fileService.DeleteFile(optimizedPath);
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

            var match = Regex.Match(text, @"\b\d{3}-?\d{7}-?\d{1}\b");
            return match.Success ? match.Value.Replace("-", "") : null;
        }
    }
}
