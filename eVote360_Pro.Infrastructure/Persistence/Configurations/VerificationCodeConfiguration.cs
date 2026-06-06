using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad VerificationCode.
    /// Define las reglas de seguridad para los códigos OTP de validación de identidad.
    /// </summary>
    public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            builder.ToTable("VerificationCodes");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Code).IsRequired().HasMaxLength(10);
            builder.Property(x => x.ExpirationDate).IsRequired();
            builder.Property(x => x.IsUsed).IsRequired().HasDefaultValue(false);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.Citizen)
                .WithMany(c => c.VerificationCodes)
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Election)
                .WithMany(e => e.VerificationCodes)
                .HasForeignKey(x => x.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
