using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.EntityConfigurations
{
    public class PoliticalAlliancesConfiguration : IEntityTypeConfiguration<PoliticalAlliances>
    {
        public void Configure(EntityTypeBuilder<PoliticalAlliances> builder)
        {
            builder.ToTable("PoliticalAlliances");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequesterPartyId)
                .IsRequired();

            builder.Property(x => x.ReceiverPartyId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(30)
                .HasConversion<string>()
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.RequesterParty)
                .WithMany()
                .HasForeignKey(x => x.RequesterPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ReceiverParty)
                .WithMany()
                .HasForeignKey(x => x.ReceiverPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
