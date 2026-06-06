using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad Role.
    /// Define los permisos y niveles de acceso autorizados en el sistema.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Relationships

            builder
                .HasMany(x => x.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Seed Data

            // Seedeamos los roles necesarios para el sistema funcionar correctamente
            builder.HasData(
                new
                {
                    Id = 1,
                    Name = SystemRoles.Administrator,
                    CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new
                {
                    Id = 2,
                    Name = SystemRoles.PoliticalLeader,
                    CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                }
            );

            #endregion
        }
    }
}
