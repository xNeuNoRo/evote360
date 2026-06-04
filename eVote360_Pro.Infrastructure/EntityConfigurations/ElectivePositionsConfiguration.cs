using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.EntityConfigurations
{
    public class ElectivePositionsConfiguration : IEntityTypeConfiguration<ElectivePositions>
    {
        public void Configure(EntityTypeBuilder<ElectivePositions> builder)
        {
            builder.ToTable("ElectivePositions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        }
    }
}
