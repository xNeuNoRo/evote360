using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad ElectivePosition.
    /// Define los cargos disponibles y garantiza la unicidad de los mismos.
    /// </summary>
    public class ElectivePositionConfiguration : IEntityTypeConfiguration<ElectivePosition>
    {
        public void Configure(EntityTypeBuilder<ElectivePosition> builder)
        {
            builder.ToTable("ElectivePositions");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // El nombre del puesto debe ser único para evitar confusiones en la boleta electoral
            builder.HasIndex(x => x.Name).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasMany(x => x.CandidatePostAssignments)
                .WithOne(a => a.Position)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Votes)
                .WithOne(v => v.Position)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
