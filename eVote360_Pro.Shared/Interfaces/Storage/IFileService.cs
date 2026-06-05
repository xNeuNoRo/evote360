using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Shared.Interfaces.Storage
{
    /// <summary>
    /// Contrato para la gestión de persistencia física de archivos (Imágenes, documentos).
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Guarda un archivo y devuelve su ruta relativa.
        /// </summary>
        Task<string> UploadFileAsync(IFormFile file, string folderName);

        /// <summary>
        /// Elimina un archivo físico del almacenamiento.
        /// </summary>
        void DeleteFile(string filePath);
    }
}
