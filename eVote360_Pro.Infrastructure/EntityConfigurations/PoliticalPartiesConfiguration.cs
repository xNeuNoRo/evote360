using eVote360_Pro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote360_Pro.Infrastructure.EntityConfigurations
{
    public class PoliticalPartiesConfiguration : IEntityTypeConfiguration<PoliticalParties>
    {
        public void Configure(EntityTypeBuilder<PoliticalParties> builder)
        {
            builder.ToTable("PoliticalParties");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)"); 

            builder.Property(x => x.Acronym)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => x.Acronym)
                .IsUnique();

            builder.Property(x => x.LogoPath)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
        }
    }
}
