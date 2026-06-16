namespace eVote360_Pro.Domain.Interfaces.Providers
{
    /// <summary>
    /// Provee una interfaz para tener centralizado el acceso a la fecha y hora actual,
    /// Si la app fuese a escalar, esto seria muy util para poder hacer test unitarios sencillos
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
