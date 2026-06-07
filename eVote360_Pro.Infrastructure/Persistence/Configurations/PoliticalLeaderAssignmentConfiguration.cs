using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configuración para la entidad PoliticalLeaderAssignment.
    /// Garantiza la relación 1:1 estricta entre el usuario dirigente y su partido político.
    /// </summary>
    public class PoliticalLeaderAssignmentConfiguration
        : IEntityTypeConfiguration<PoliticalLeaderAssignment>
    {
        public void Configure(EntityTypeBuilder<PoliticalLeaderAssignment> builder)
        {
            builder.ToTable("PoliticalLeaderAssignments");

            // En esta relacion 1:1, el UserId (Guid) actúa como PK y FK al mismo tiempo
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired(false);

            #endregion

            #region Indexes

            // Un partido solo puede tener UN dirigente asignado
            builder.HasIndex(x => x.PartyId).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.User)
                .WithOne(u => u.LeaderAssignment)
                .HasForeignKey<PoliticalLeaderAssignment>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Party)
                .WithOne(p => p.LeaderAssignment)
                .HasForeignKey<PoliticalLeaderAssignment>(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
