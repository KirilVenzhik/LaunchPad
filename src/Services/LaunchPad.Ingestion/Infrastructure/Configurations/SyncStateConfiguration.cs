using LaunchPad.Ingestion.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaunchPad.Ingestion.Infrastructure.Configurations;

public class SyncStateConfiguration : IEntityTypeConfiguration<SyncState>
{
    public void Configure(EntityTypeBuilder<SyncState> builder)
    {
        builder.ToTable("sync_state");
        builder.HasKey(x => x.Ll2Id);                       // natural key = LL2 uuid
        builder.Property(x => x.ContentHash).HasColumnType("bytea");
        builder.Property(x => x.LastSnapshotJson).HasColumnType("jsonb");
    }
}
