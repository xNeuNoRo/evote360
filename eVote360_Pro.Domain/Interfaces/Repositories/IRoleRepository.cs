using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Contrato para la persistencia y consulta de los roles del sistema (Administrador, Dirigente Político).
    /// </summary>
    public interface IRoleRepository : IGenericRepository<Role, int>
    {
        /// <summary>
        /// Obtiene un rol por su nombre exacto. 
        /// </summary>
        Task<Role?> GetByNameAsync(string roleName);

        /// <summary>
        /// Verifica si ya existe un rol con el mismo nombre.
        /// </summary>
        Task<bool> ExistsByNameAsync(string roleName, int? excludeId = null);
    }
}
