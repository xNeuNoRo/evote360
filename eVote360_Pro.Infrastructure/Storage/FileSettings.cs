namespace eVote360_Pro.Infrastructure.Storage
{
    /// <summary>
    /// Configuración para el servicio de almacenamiento de archivos.
    /// </summary>
    public class FileSettings
    {
        public const string SectionName = "FileSettings";

        /// <summary>
        /// Ruta base de almacenamiento.
        /// </summary>
        public string BasePath { get; init; } = null!;

        /// <summary>
        /// Prefijo de la URL relativa (ej: "/uploads").
        /// </summary>
        public string UrlPrefix { get; init; } = "/uploads";

        /// <summary>
        /// Tamaño máximo permitido en bytes (por defecto 15MB).
        /// </summary>
        public long MaxSizeInBytes { get; init; } = 15 * 1024 * 1024;
    }
}
