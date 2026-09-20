using LaunchPad.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaunchPad.Catalog.Infrastructure.Configurations;

public class LaunchConfiguration : IEntityTypeConfiguration<Launch>
{
    public void Configure(EntityTypeBuilder<Launch> builder)
    {
        builder.ToTable("launches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<short>();
        builder.Property(x => x.MissionOrbit).HasConversion<short>();

        builder.HasIndex(x => x.Ll2Id).IsUnique();
        builder.HasIndex(x => x.NetUtc);
        builder.HasIndex(x => new { x.Status, x.NetUtc });

        builder.HasOne(x => x.Agency).WithMany()
            .HasForeignKey(x => x.AgencyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RocketConfig).WithMany()
            .HasForeignKey(x => x.RocketConfigId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Pad).WithMany()
            .HasForeignKey(x => x.PadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.History).WithOne()
            .HasForeignKey(h => h.LaunchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();
    }
}
