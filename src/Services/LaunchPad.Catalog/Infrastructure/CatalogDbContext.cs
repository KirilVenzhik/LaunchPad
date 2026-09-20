using LaunchPad.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LaunchPad.Catalog.Infrastructure;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Launch> Launches => Set<Launch>();
    public DbSet<Agency> Agencies => Set<Agency>();
    public DbSet<RocketConfig> RocketConfigs => Set<RocketConfig>();
    public DbSet<Pad> Pads => Set<Pad>();
    public DbSet<LaunchStatusHistory> LaunchStatusHistories => Set<LaunchStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
