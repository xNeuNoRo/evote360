using eVote360_Pro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eVote360_Pro.Infrastructure.Storage
{
    /// <summary>
    /// Implementación para la gestión de archivos locales.
    /// Proporciona seguridad, validación de integridad y manejo de rutas automatizado.
    /// </summary>
    public class FileService : IFileService
    {
        private readonly FileSettings _settings;
        private readonly ILogger<FileService> _logger;

        public FileService(IOptions<FileSettings> options, ILogger<FileService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            // Limpiamos y normalizamos la ruta relativa
            string cleanRelativePath = relativePath.TrimStart('/', '\\');

            // Obtenemos la ruta absoluta base
            string baseAbsolutePath = Path.GetFullPath(_settings.BasePath);

            // Combinamos y resolvemos la ruta completa
            string combinedPath = Path.GetFullPath(
                Path.Combine(baseAbsolutePath, cleanRelativePath)
            );

            // La ruta final DEBE empezar por la ruta base configurada para evitar Path Traversal
            if (!combinedPath.StartsWith(baseAbsolutePath, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogCritical(
                    "Intento de acceso ilegal fuera del directorio base (Path Traversal): {Path}",
                    combinedPath
                );
                return string.Empty;
            }

            return combinedPath;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                // Sanitizamos el nombre de la carpeta para evitar
                // caracteres inválidos o intentos de path traversal
                string sanitizedFolder = string.Join(
                        "_",
                        folderName.Split(Path.GetInvalidFileNameChars())
                    )
                    .Replace("..", "")
                    .Trim('/', '\\');

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Construimos rutas (Absoluta para Guardar, Relativa para la DB)
                string uploadRoot = Path.Combine(
                    _settings.UrlPrefix.TrimStart('/', '\\'),
                    sanitizedFolder
                );
                string absoluteFolderPath = GetAbsolutePath(uploadRoot);

                if (string.IsNullOrEmpty(absoluteFolderPath))
                {
                    throw new InvalidOperationException(
                        "La ruta de carga no es válida o es insegura."
                    );
                }

                string relativePath =
                    $"/{_settings.UrlPrefix.Trim('/')}/{sanitizedFolder}/{fileName}";

                // Aseguramos que el directorio existe
                if (!Directory.Exists(absoluteFolderPath))
                {
                    Directory.CreateDirectory(absoluteFolderPath);
                }

                // Guardamos el archivo de forma asíncrona
                string absoluteFilePath = Path.Combine(absoluteFolderPath, fileName);
                using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Archivo guardado exitosamente: {Path}", relativePath);
                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al intentar guardar archivo permanente.");
                throw;
            }
        }

        public async Task<string> UploadTempFileAsync(IFormFile file)
        {
            try
            {
                // Generamos un nombre único para el archivo temporal
                string fileName = $"temp_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Usamos GetAbsolutePath para asegurar que la carpeta 'temp' sea segura
                string absoluteTempPath = GetAbsolutePath("temp");

                if (string.IsNullOrEmpty(absoluteTempPath))
                {
                    throw new InvalidOperationException("El directorio temporal no es seguro.");
                }

                if (!Directory.Exists(absoluteTempPath))
                {
                    Directory.CreateDirectory(absoluteTempPath);
                }

                // Guardamos el archivo temporal de forma asíncrona
                string fullPath = Path.Combine(absoluteTempPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retornamos la ruta relativa para su uso posterior
                return "temp/" + fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar archivo temporal para OCR.");
                throw;
            }
        }

        public bool IsImageValid(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Validamos el size del archivo
            if (file.Length > _settings.MaxSizeInBytes)
            {
                _logger.LogWarning(
                    "Intento de carga de imagen excediendo tamaño: {Size} bytes",
                    file.Length
                );
                return false;
            }

            // Validamos la extensión del archivo
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Extensión de archivo no permitida: {Extension}", extension);
                return false;
            }

            // Validamos el tipo MIMETYPE del archivo
            string mimeType = file.ContentType.ToLowerInvariant();
            if (!_settings.AllowedMimeTypes.Contains(mimeType))
            {
                _logger.LogWarning("MIMETYPE no permitido: {MimeType}", mimeType);
                return false;
            }

            return true;
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return;

                string absolutePath = GetAbsolutePath(filePath);

                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                    _logger.LogInformation("Archivo eliminado del servidor: {Path}", absolutePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo eliminar el archivo físico: {Path}.", filePath);
            }
        }
    }
}
