using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de roles.
    /// Proporciona acceso semántico a las definiciones de seguridad del sistema.
    /// </summary>
    public class RoleRepository : GenericRepository<Role, int>, IRoleRepository
    {
        public RoleRepository(AppDbContext context)
            : base(context) { }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<bool> ExistsByNameAsync(string roleName, int? excludeId = null)
        {
            string normalized = roleName.Trim();
            return await _dbSet.AnyAsync(r => r.Name == normalized && r.Id != excludeId);
        }
    }
}
