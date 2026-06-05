namespace eVote360_Pro.Domain.Common
{
    /// <summary>
    /// Clase base para todas las entidades del dominio con soporte para identificadores genéricos.
    /// Las propiedades de auditoría son gestionadas automáticamente por la infraestructura.
    /// </summary>
    /// <typeparam name="TId">El tipo de dato del identificador (ej: int, Guid).</typeparam>
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; protected set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Versión simplificada de BaseEntity que utiliza int como identificador por defecto.
    /// </summary>
    public abstract class BaseEntity : BaseEntity<int> { }
}
