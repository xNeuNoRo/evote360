using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad Candidate.
    /// Define las reglas de identidad de los candidatos y su vinculación con los partidos.
    /// </summary>
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("Candidates");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PhotoPath).IsRequired(); // La foto es vital para que el elector reconozca al candidato en la boleta
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            // Campos de auditoria (el dbcontext lo resuelve solo xd)
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Relationships


            builder
                .HasOne(x => x.OriginalParty)
                .WithMany(p => p.Candidates)
                .HasForeignKey(x => x.OriginalPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.PostAssignments)
                .WithOne(a => a.Candidate)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Votes)
                .WithOne(v => v.Candidate)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
