using System.Reflection;
using LaunchPad.Ingestion.Domain;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Ingestion.Infrastructure;

public class IngestionDbContext(DbContextOptions<IngestionDbContext> options) : DbContext(options)
{
    public DbSet<SyncState> SyncStates => Set<SyncState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddTransactionalOutboxEntities();   // MassTransit outbox/inbox таблицы
        base.OnModelCreating(modelBuilder);
    }
}
