using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad Vote.
    /// Garantiza el anonimato y la integridad del sufragio.
    /// </summary>
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.ToTable("Votes");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            // Los votos son nulos si no tienen candidato ni partido asociado, lo que nos permite el voto en blanco
            builder.Property(x => x.CandidateId).IsRequired(false);
            builder.Property(x => x.PartyId).IsRequired(false);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.Election)
                .WithMany(e => e.Votes)
                .HasForeignKey(x => x.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Position)
                .WithMany(p => p.Votes)
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Candidate)
                .WithMany(c => c.Votes)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Party)
                .WithMany(p => p.Votes)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
