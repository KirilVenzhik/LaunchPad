using LaunchPad.Ingestion.Acl;
using LaunchPad.Ingestion.Infrastructure;
using LaunchPad.Ingestion.Sync;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// --- Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Services.AddSerilog();

// --- EF Core (PostgreSQL, snake_case) ---
builder.Services.AddDbContext<IngestionDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("IngestionDb"))
     .UseSnakeCaseNamingConvention());

// --- LL2 typed client + resilience (retry/timeout/circuit-breaker) ---
builder.Services.AddHttpClient<ILaunchLibraryClient, LaunchLibraryClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["LaunchLibrary:BaseUrl"]!);
    c.Timeout = TimeSpan.FromSeconds(30);
})
.AddStandardResilienceHandler();

// --- MassTransit + EF transactional outbox (publish-only) ---
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<IngestionDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<SyncService>();
builder.Services.AddHostedService<IngestionSyncWorker>();

var host = builder.Build();
host.Run();
