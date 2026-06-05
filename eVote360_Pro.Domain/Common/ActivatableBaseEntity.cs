namespace eVote360_Pro.Domain.Common
{
    /// <summary>
    /// Clase base para entidades que soportan borrado lógico (activación/desactivación) con identificadores genéricos.
    /// </summary>
    /// <typeparam name="TId">El tipo de dato del identificador.</typeparam>
    public abstract class ActivatableBaseEntity<TId> : BaseEntity<TId>
    {
        public bool IsActive { get; protected set; } = true;
    }

    /// <summary>
    /// Versión simplificada de ActivatableBaseEntity que utiliza int como identificador por defecto.
    /// </summary>
    public abstract class ActivatableBaseEntity : ActivatableBaseEntity<int> { }
}
