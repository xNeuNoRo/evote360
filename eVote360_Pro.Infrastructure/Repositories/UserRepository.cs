using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de usuarios.
    /// Gestiona la seguridad, autenticación y protección del administrador del sistema.
    /// </summary>
    public class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context) { }

        public async Task<User?> GetUserWithSecurityDetailsAsync(string username)
        {
            // Cargamos el usuario con su Rol y Asignación de Partido en una sola consulta
            return await _dbSet
                .Include(u => u.Role)
                .Include(u => u.LeaderAssignment)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower().Trim());
        }

        public async Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null)
        {
            string normalized = username.ToLowerInvariant().Trim();
            return await _dbSet.AnyAsync(u =>
                u.Username.ToLower() == normalized && u.Id != excludeId
            );
        }

        public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
        {
            string normalized = email.ToLowerInvariant().Trim();
            return await _dbSet.AnyAsync(u => u.Email.ToLower() == normalized && u.Id != excludeId);
        }

        public async Task<bool> IsLastActiveAdminAsync(Guid userId)
        {
            // Buscamos el rol de Administrador
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r =>
                r.Name == SystemRoles.Administrator
            );
            if (adminRole == null)
                return false;

            // Contamos cuántos administradores activos existen
            int activeAdminsCount = await _dbSet.CountAsync(u =>
                u.RoleId == adminRole.Id && u.IsActive
            );

            // Si solo hay uno y es el usuario consultado, entonces es el último activo
            if (activeAdminsCount == 1)
            {
                return await _dbSet.AnyAsync(u =>
                    u.Id == userId && u.RoleId == adminRole.Id && u.IsActive
                );
            }

            return false;
        }
    }
}
