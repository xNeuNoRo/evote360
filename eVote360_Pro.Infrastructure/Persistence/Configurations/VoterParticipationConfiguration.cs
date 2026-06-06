using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad VoterParticipation.
    /// Garantiza físicamente la regla de un solo voto por ciudadano en cada elección.
    /// </summary>
    public class VoterParticipationConfiguration : IEntityTypeConfiguration<VoterParticipation>
    {
        public void Configure(EntityTypeBuilder<VoterParticipation> builder)
        {
            builder.ToTable("VoterParticipations");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // Indice unico para garantizar que un ciudadano solo pueda votar una vez por elección
            builder.HasIndex(x => new { x.CitizenId, x.ElectionId }).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.Citizen)
                .WithMany(c => c.Participations)
                .HasForeignKey(x => x.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Election)
                .WithMany(e => e.Participations)
                .HasForeignKey(x => x.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
