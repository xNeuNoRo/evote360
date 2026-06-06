using eVote360_Pro.Shared.Interfaces.Storage;
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
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png" };

        public FileService(IOptions<FileSettings> options, ILogger<FileService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            // Quitamos el slash inicial para que Path.Combine funcione correctamente
            string cleanRelativePath = relativePath.TrimStart('/');
            // Si la ruta ya incluye el prefijo de uploads, debemos mapearla correctamente al BasePath
            return Path.Combine(_settings.BasePath, cleanRelativePath);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                // Generamos un nombre único y sanitizar carpeta
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string subFolder = folderName.Trim('/', '\\');

                // Construimos rutas (Absoluta para Guardar, Relativa para la DB)
                string absoluteFolderPath = Path.Combine(
                    _settings.BasePath,
                    _settings.UrlPrefix.TrimStart('/'),
                    subFolder
                );
                string relativePath = $"/{_settings.UrlPrefix.Trim('/')}/{subFolder}/{fileName}";

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
                // Construimos la ruta absoluta para el archivo temporal
                string absoluteTempPath = Path.Combine(_settings.BasePath, "temp");

                // Aseguramos que el directorio temporal existe
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

                return fullPath;
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
            if (!_allowedExtensions.Contains(extension))
                return false;

            // Validamos el tipo MIMETYPE del archivo
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return;

                // Construimos la ruta absoluta desde la relativa de la DB
                // Ej: /uploads/candidates/abc.jpg -> C:/App/folder/uploads/candidates/abc.jpg
                string relativePath = filePath.TrimStart('/');
                string absolutePath = Path.Combine(_settings.BasePath, relativePath);

                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                    _logger.LogInformation("Archivo eliminado del servidor: {Path}", absolutePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "No se pudo eliminar el archivo físico: {Path}. Es posible que esté en uso.",
                    filePath
                );
            }
        }
    }
}
