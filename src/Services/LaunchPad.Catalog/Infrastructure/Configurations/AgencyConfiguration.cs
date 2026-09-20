using LaunchPad.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaunchPad.Catalog.Infrastructure.Configurations;

public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        builder.ToTable("agencies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<short>();
        builder.Property(x => x.CountryCode).HasMaxLength(2).IsFixedLength();
        builder.HasIndex(x => x.Ll2Id).IsUnique();
        builder.HasIndex(x => x.CountryCode);
    }
}
