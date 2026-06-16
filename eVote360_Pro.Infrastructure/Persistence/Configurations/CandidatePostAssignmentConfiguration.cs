using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad CandidatePostAssignment.
    /// Define la estructura de la boleta electoral y las reglas de asignación propias/aliadas.
    /// </summary>
    public class CandidatePostAssignmentConfiguration
        : IEntityTypeConfiguration<CandidatePostAssignment>
    {
        public void Configure(EntityTypeBuilder<CandidatePostAssignment> builder)
        {
            builder.ToTable("CandidatePostAssignments");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.IsAlly).IsRequired().HasDefaultValue(false);

            // Auditoria q el dbcontext resuelve solo
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // Un mismo puesto no puede ser ocupado por dos candidatos del mismo partido
            builder.HasIndex(x => new { x.PositionId, x.PartyId }).IsUnique();
            // Un mismo candidato no puede postularse para el mismo puesto con el mismo partido más de una vez
            builder.HasIndex(x => new { x.CandidateId, x.PartyId }).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.Candidate)
                .WithMany(c => c.PostAssignments)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Position)
                .WithMany(p => p.CandidatePostAssignments)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Party)
                .WithMany(p => p.CandidatePostAssignments)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
