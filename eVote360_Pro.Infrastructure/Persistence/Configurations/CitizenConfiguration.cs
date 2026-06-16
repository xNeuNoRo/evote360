using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad Citizen.
    /// Implementa las reglas de identidad única y auditoría del padrón electoral.
    /// </summary>
    public class CitizenConfiguration : IEntityTypeConfiguration<Citizen>
    {
        public void Configure(EntityTypeBuilder<Citizen> builder)
        {
            builder.ToTable("Citizens");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.IdentityDocument).IsRequired().HasMaxLength(20);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            // Props de auditoria basica
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // Indice unico para evitar duplicados de cedulas
            builder.HasIndex(x => x.IdentityDocument).IsUnique();

            // Indice unico para evitar duplicados de emails
            builder.HasIndex(x => x.Email).IsUnique();

            #endregion

            #region Relationships

            // Relacion con los codigos OTP, si se borra el ciudadano, se borran sus codigos OTP asociados
            builder
                .HasMany(x => x.VerificationCodes)
                .WithOne(x => x.Citizen)
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacion con las participaciones, si se borra el ciudadano, no se borran sus participaciones
            builder
                .HasMany(x => x.Participations)
                .WithOne(x => x.Citizen)
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
