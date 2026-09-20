using LaunchPad.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaunchPad.Catalog.Infrastructure.Configurations;

public class PadConfiguration : IEntityTypeConfiguration<Pad>
{
    public void Configure(EntityTypeBuilder<Pad> builder)
    {
        builder.ToTable("pads");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CountryCode).HasMaxLength(2).IsFixedLength();
        builder.HasIndex(x => x.Ll2Id).IsUnique();
    }
}
