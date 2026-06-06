using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad PoliticalParty.
    /// Define las reglas de identidad política y las relaciones de alianzas del sistema.
    /// </summary>
    public class PoliticalPartyConfiguration : IEntityTypeConfiguration<PoliticalParty>
    {
        public void Configure(EntityTypeBuilder<PoliticalParty> builder)
        {
            builder.ToTable("PoliticalParties");
            builder.HasKey(x => x.Id);

            #region Properties Configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Acronym).IsRequired().HasMaxLength(20);
            builder.Property(x => x.LogoPath).IsRequired(); // El logo es obligatorio para identificar al partido en la boleta
            builder.Property(x => x.Description).HasMaxLength(500).IsRequired(false);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            // Campos de auditoria (se llenan solos en el DbContext)
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // Si un partido tiene "PLD no puede haber otro obviamente q confunda al elector
            builder.HasIndex(x => x.Acronym).IsUnique();
            // Lo mismo con el nombre
            builder.HasIndex(x => x.Name).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.LeaderAssignment)
                .WithOne(x => x.Party)
                .HasForeignKey<PoliticalLeaderAssignment>(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.RequestedAlliances)
                .WithOne(x => x.RequesterParty)
                .HasForeignKey(x => x.RequesterPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.ReceivedAlliances)
                .WithOne(x => x.ReceiverParty)
                .HasForeignKey(x => x.ReceiverPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Candidates)
                .WithOne(x => x.OriginalParty)
                .HasForeignKey(x => x.OriginalPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasMany(x => x.Votes)
                .WithOne(x => x.Party)
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
