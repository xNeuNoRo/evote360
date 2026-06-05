using System.Text.RegularExpressions;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;

namespace eVote360_Pro.Domain.Entities
{
    /// <summary>
    /// Representa un usuario administrativo o político con acceso al sistema.
    /// Encapsula la lógica de seguridad, roles y restricciones de desactivación.
    /// </summary>
    public class User : ActivatableBaseEntity
    {
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Username { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public int RoleId { get; private set; }

        // Navigation properties
        public virtual Role? Role { get; private set; }
        public virtual PoliticalLeaderAssignment? LeaderAssignment { get; private set; }

        // Constructor privado para EF Core y Factory Method
        private User() { }

        /// <summary>
        /// Crea un nuevo usuario con validaciones básicas de identidad y formato.
        /// </summary>
        public static User Create(
            string firstName,
            string lastName,
            string email,
            string username,
            string passwordHash,
            int roleId
        )
        {
            ValidateBasicInfo(firstName, lastName, email, username);

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("La contraseña es requerida.", "User.PasswordRequired");

            if (roleId <= 0)
                throw new DomainException("Debe seleccionar un rol válido.", "User.InvalidRole");

            return new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                Username = username.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                RoleId = roleId,
                IsActive = true,
            };
        }

        /// <summary>
        /// Actualiza la información básica del usuario.
        /// </summary>
        public void UpdateInformation(
            string firstName,
            string lastName,
            string email,
            string username
        )
        {
            ValidateBasicInfo(firstName, lastName, email, username);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            Username = username.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Actualiza el rol del usuario validando restricciones de vinculación política.
        /// </summary>
        public void UpdateRole(int newRoleId, bool hasAssignedParty, bool isLastAdmin)
        {
            if (RoleId == newRoleId)
                return;

            if (hasAssignedParty)
                throw new DomainException(
                    "No se puede cambiar el rol porque tiene un partido político asignado como dirigente.",
                    "User.RoleChangeBlockedByParty"
                );

            if (isLastAdmin)
                throw new DomainException(
                    "No se puede modificar este usuario porque es el único administrador activo del sistema.",
                    "User.LastAdminRoleChangeBlocked"
                );

            RoleId = newRoleId;
        }

        /// <summary>
        /// Actualiza la contraseña hash del usuario.
        /// </summary>
        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException(
                    "La nueva contraseña no puede estar vacía.",
                    "User.PasswordRequired"
                );

            PasswordHash = newPasswordHash;
        }

        /// <summary>
        /// Desactiva al usuario validando restricciones de seguridad y el estado del administrador.
        /// </summary>
        public void Deactivate(bool isLastAdmin, bool isSelf)
        {
            if (isSelf)
                throw new DomainException(
                    "No puede desactivar su propio usuario mientras está autenticado.",
                    "User.SelfDeactivationBlocked"
                );

            if (isLastAdmin)
                throw new DomainException(
                    "No se puede desactivar este usuario porque es el único administrador activo del sistema.",
                    "User.LastAdminDeactivationBlocked"
                );

            IsActive = false;
        }

        /// <summary>
        /// Activa el usuario.
        /// </summary>
        public void Activate()
        {
            IsActive = true;
        }

        private static void ValidateBasicInfo(
            string firstName,
            string lastName,
            string email,
            string username
        )
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("El nombre es requerido.", "User.FirstNameRequired");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("El apellido es requerido.", "User.LastNameRequired");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException(
                    "El correo electrónico es requerido.",
                    "User.EmailRequired"
                );

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new DomainException(
                    "El correo electrónico debe tener un formato válido.",
                    "User.InvalidEmailFormat"
                );

            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException(
                    "El nombre de usuario es requerido.",
                    "User.UsernameRequired"
                );
        }
    }
}
