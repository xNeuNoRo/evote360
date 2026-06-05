namespace eVote360_Pro.Shared.Interfaces.OCR
{
    /// <summary>
    /// Encapsula el resultado del procesamiento de una imagen de identificación.
    /// Proporciona datos sobre la extracción, validación y confianza del motor OCR/IA.
    /// </summary>
    public class OCRResponse
    {
        /// <summary>
        /// Indica si el proceso se completó exitosamente.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// El número de identidad extraído (nulo si falló).
        /// </summary>
        public string? IdentityNumber { get; set; }

        /// <summary>
        /// Indica si el formato de la imagen coincide con un documento de identidad válido.
        /// Útil para detectar si se subió un documento incorrecto o inválido.
        /// </summary>
        public bool IsDocumentValid { get; set; }

        /// <summary>
        /// Nivel de confianza del motor en la extracción (0.0 a 1.0).
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// Mensaje descriptivo del error en caso de que IsSuccess sea false.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
