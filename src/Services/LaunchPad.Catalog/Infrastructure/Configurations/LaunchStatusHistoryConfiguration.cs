using LaunchPad.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaunchPad.Catalog.Infrastructure.Configurations;

public class LaunchStatusHistoryConfiguration : IEntityTypeConfiguration<LaunchStatusHistory>
{
    public void Configure(EntityTypeBuilder<LaunchStatusHistory> builder)
    {
        builder.ToTable("launch_status_history");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChangeType).HasConversion<short>();
        builder.Property(x => x.OldStatus).HasConversion<short>();
        builder.Property(x => x.NewStatus).HasConversion<short>();
        builder.Property(x => x.LaunchId);
    }
}
