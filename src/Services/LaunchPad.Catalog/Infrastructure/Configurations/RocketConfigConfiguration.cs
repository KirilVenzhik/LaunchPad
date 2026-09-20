using LaunchPad.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaunchPad.Catalog.Infrastructure.Configurations;

public class RocketConfigConfiguration : IEntityTypeConfiguration<RocketConfig>
{
    public void Configure(EntityTypeBuilder<RocketConfig> builder)
    {
        builder.ToTable("rocket_configs");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Ll2Id).IsUnique();
        builder.HasIndex(x => x.Family);
    }
}
