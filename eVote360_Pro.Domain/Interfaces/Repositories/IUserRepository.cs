using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y consultas específicas de seguridad para los usuarios.
    /// Encapsula las reglas de autenticación y protección del administrador del sistema.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User, Guid>
    {
        /// <summary>
        /// Obtiene un usuario por su nombre de usuario, incluyendo su Rol y Asignación Política.
        /// </summary>
        Task<User?> GetUserWithSecurityDetailsAsync(string username);

        /// <summary>
        /// Verifica si un nombre de usuario ya está registrado en el sistema.
        /// Permite excluir un ID específico para validaciones en procesos de edición.
        /// </summary>
        Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null);

        /// <summary>
        /// Verifica si un correo electrónico ya está registrado en el sistema.
        /// Permite excluir un ID específico para validaciones en procesos de edición.
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);

        /// <summary>
        /// Verifica si el usuario especificado es el único administrador activo del sistema.
        /// </summary>
        Task<bool> IsLastActiveAdminAsync(Guid userId);
    }
}
