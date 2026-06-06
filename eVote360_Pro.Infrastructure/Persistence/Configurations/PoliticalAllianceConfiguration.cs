using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad PoliticalAlliance.
    /// Define las relaciones de pactos entre partidos y sus estados de aprobación.
    /// </summary>
    public class PoliticalAllianceConfiguration : IEntityTypeConfiguration<PoliticalAlliance>
    {
        public void Configure(EntityTypeBuilder<PoliticalAlliance> builder)
        {
            builder.ToTable("PoliticalAlliances");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            // Guardamos el enum como string para legibilidad en la BD
            builder.Property(x => x.Status).IsRequired().HasConversion<string>();
            builder.Property(x => x.AcceptedAt).IsRequired(false);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // Indice para buscar rapidamente las alianzas entre dos partidos específicos
            builder.HasIndex(x => new { x.RequesterPartyId, x.ReceiverPartyId });

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.RequesterParty)
                .WithMany(p => p.RequestedAlliances)
                .HasForeignKey(x => x.RequesterPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.ReceiverParty)
                .WithMany(p => p.ReceivedAlliances)
                .HasForeignKey(x => x.ReceiverPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
