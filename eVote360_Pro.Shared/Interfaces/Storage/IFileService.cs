using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Shared.Interfaces.Storage
{
    /// <summary>
    /// Contrato para la gestión avanzada de persistencia física de archivos.
    /// Encapsula validaciones de seguridad, integridad y manejo de temporales.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Guarda un archivo de forma permanente en el servidor con un nombre único.
        /// </summary>
        /// <param name="file">El archivo recibido desde la WebApp.</param>
        /// <param name="folderName">Subcarpeta de destino (ej: "candidates", "parties").</param>
        /// <returns>La ruta relativa lista para ser guardada en la base de datos.</returns>
        Task<string> UploadFileAsync(IFormFile file, string folderName);

        /// <summary>
        /// Guarda un archivo de forma temporal para procesamiento inmediato (ej: OCR).
        /// </summary>
        /// <param name="file">El archivo de identidad del ciudadano.</param>
        /// <returns>La ruta absoluta del archivo temporal en el servidor.</returns>
        Task<string> UploadTempFileAsync(IFormFile file);

        /// <summary>
        /// Valida si un archivo es una imagen permitida (.jpg, .png) y cumple con las reglas de integridad.
        /// </summary>
        /// <param name="file">Archivo a validar.</param>
        /// <returns>True si es una imagen válida según los requerimientos técnicos.</returns>
        bool IsImageValid(IFormFile file);

        /// <summary>
        /// Elimina un archivo físico del almacenamiento.
        /// </summary>
        /// <param name="filePath">Ruta relativa o absoluta del archivo.</param>
        void DeleteFile(string filePath);
    }
}
