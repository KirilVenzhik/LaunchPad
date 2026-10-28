using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LaunchPad.Ingestion.Infrastructure;

// Design-time фабрика: dotnet ef создаёт контекст напрямую,
// не поднимая хост/MassTransit. Строка — только для миграций (dev).
public class IngestionDbContextFactory : IDesignTimeDbContextFactory<IngestionDbContext>
{
    public IngestionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IngestionDbContext>()
            .UseNpgsql("Host=localhost;Database=launch_pad_ingestion;Username=postgres;Password=root")
            .UseSnakeCaseNamingConvention()
            .Options;

        return new IngestionDbContext(options);
    }
}
