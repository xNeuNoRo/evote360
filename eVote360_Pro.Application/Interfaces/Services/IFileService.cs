using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Contrato para la gestión avanzada de persistencia física de archivos.
    /// </summary>
    public interface IFileService
    {
        string GetAbsolutePath(string relativePath);

        Task<string> UploadFileAsync(IFormFile file, string folderName);

        Task<string> UploadTempFileAsync(IFormFile file);

        bool IsImageValid(IFormFile file);

        void DeleteFile(string filePath);
    }
}
