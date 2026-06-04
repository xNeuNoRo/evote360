using eVote360_Pro.Domain.Interfaces.Providers;

namespace eVote360_Pro.Domain.Providers
{
    /// <summary>
    /// Implementación de IDateTimeProvider que devuelve la fecha y hora actual en formato UTC.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Obtiene la fecha y hora actual en formato UTC.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
