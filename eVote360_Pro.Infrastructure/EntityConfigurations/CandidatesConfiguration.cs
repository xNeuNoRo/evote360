using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.EntityConfigurations
{
    public class CandidatesConfiguration : IEntityTypeConfiguration<Candidates>
    {
        public void Configure(EntityTypeBuilder<Candidates> builder)
        {
            builder.ToTable("Candidates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.PhotoPath)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasOne(x => x.OriginalParty)
                .WithMany()
                .HasForeignKey(x => x.OriginalPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
