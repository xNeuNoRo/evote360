using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.EntityConfigurations
{
    public class CandidatePostAssignmentsConfiguration : IEntityTypeConfiguration<CandidatePostAssignments>
    {
        public void Configure(EntityTypeBuilder<CandidatePostAssignments> builder)
        {
            builder.ToTable("CandidatePostAssignments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CandidateId)
                .IsRequired();

            builder.Property(x => x.PositionId)
                .IsRequired();

            builder.Property(x => x.PartyId)
                .IsRequired();

            builder.Property(x => x.IsAlly)
                .HasDefaultValue(false);

            // Indexes as per DBML
            builder.HasIndex(x => new { x.CandidateId, x.PartyId })
                .IsUnique();

            builder.HasIndex(x => new { x.PositionId, x.PartyId })
                .IsUnique();

            // Relationships
            builder.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Position)
                .WithMany()
                .HasForeignKey(x => x.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Party)
                .WithMany()
                .HasForeignKey(x => x.PartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
