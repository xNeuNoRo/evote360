using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad Election.
    /// Controla el ciclo de vida de los procesos electorales y asegura su integridad.
    /// </summary>
    public class ElectionConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            builder.ToTable("Elections");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.RealizationDate).IsRequired();
            builder.Property(x => x.Status).IsRequired().HasConversion<string>(); // Guardamos como string para ver "Active", "Pending" en la BD
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true); // Solo una elección puede estar activa a la vez, pero esto se controla a nivel de la application

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Relationships

            builder
                .HasMany(x => x.Votes)
                .WithOne(v => v.Election)
                .HasForeignKey(x => x.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Participations)
                .WithOne(p => p.Election)
                .HasForeignKey(x => x.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.VerificationCodes)
                .WithOne(c => c.Election)
                .HasForeignKey(x => x.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
