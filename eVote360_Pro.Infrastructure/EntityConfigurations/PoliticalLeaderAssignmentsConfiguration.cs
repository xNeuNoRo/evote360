using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.EntityConfigurations
{
    public class PoliticalLeaderAssignmentsConfiguration : IEntityTypeConfiguration<PoliticalLeaderAssignments>
    {
        public void Configure(EntityTypeBuilder<PoliticalLeaderAssignments> builder)
        {
            builder.ToTable("PoliticalLeaderAssignments");

            // Map Id from BaseEntity to UserId column in the database
            // and set it as the Primary Key as per DBML [pk]
            builder.Property(x => x.Id)
                .HasColumnName("UserId")
                .ValueGeneratedNever();

            builder.HasKey(x => x.Id);

            // Ignore the alias property to avoid duplicate mapping
            builder.Ignore(x => x.UserId);

            builder.Property(x => x.PartyId)
                .IsRequired();

            // Unique constraints as per DBML [unique]
            // The PK is already unique, but we can explicitly add a unique index if desired.
            builder.HasIndex(x => x.Id)
                .IsUnique();

            builder.HasIndex(x => x.PartyId)
                .IsUnique();

            // Relationships
            builder.HasOne(x => x.Party)
                .WithMany()
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        }
    }
}
