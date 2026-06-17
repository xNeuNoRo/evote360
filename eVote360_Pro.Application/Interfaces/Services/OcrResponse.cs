namespace eVote360_Pro.Application.Interfaces.Services
{
    /// <summary>
    /// Encapsula el resultado del procesamiento de una imagen de identificación.
    /// </summary>
    public class OcrResponse
    {
        public bool IsSuccess { get; set; }

        public string? IdentityNumber { get; set; }

        public bool IsDocumentValid { get; set; }

        public float Confidence { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
