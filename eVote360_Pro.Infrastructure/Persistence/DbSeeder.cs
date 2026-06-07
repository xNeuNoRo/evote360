using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Security;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace eVote360_Pro.Infrastructure.Persistence
{
    /// <summary>
    /// Seeder dinámico para inicializar datos que requieren lógica (como hashing de passwords).
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Crea el usuario administrador inicial si el sistema está vacío.
        /// </summary>
        public static async Task SeedAdminUserAsync(
            AppDbContext context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration
        )
        {
            // Solo actuamos si no existen usuarios en el sistema
            if (await context.Users.AnyAsync())
                return;

            // Obtenemos el ID del rol Administrador para asignarlo al usuario
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Administrador");

            if (adminRole == null)
                return;

            // Generamos el hash para la contraseña por defecto
            string defaultPassword = configuration["SeedData:AdminPassword"] ?? "Admin123!";
            string passwordHash = passwordHasher.Hash(defaultPassword);

            // Creamos el usuario inicial
            var adminUser = User.Create(
                "Super",
                "Administrador",
                "admin@evote360.pro",
                "admin",
                passwordHash,
                adminRole.Id
            );

            // Persistimos en la bd
            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
