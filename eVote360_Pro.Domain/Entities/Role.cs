using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un rol de usuario en el sistema (Administrador o Dirigente Político).
    /// </summary>
    public class Role : BaseEntity
    {
        public string Name { get; private set; } = null!;

        // Navigation Properties
        public virtual ICollection<User> Users { get; private set; } = new List<User>();

        // Constructor privado para EF Core y Factory Method
        private Role() { }

        /// <summary>
        /// Crea una nueva instancia de un rol con validaciones básicas.
        /// </summary>
        public static Role Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("El nombre del rol es requerido.", "Role.NameRequired");

            return new Role { Name = name.Trim() };
        }

        /// <summary>
        /// Actualiza el nombre del rol.
        /// </summary>
        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("El nombre del rol es requerido.", "Role.NameRequired");

            Name = name.Trim();
        }
    }
}
