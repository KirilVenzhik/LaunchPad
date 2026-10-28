namespace LaunchPad.Ingestion.Sync;

// Фоновый воркер: запускает синк сразу при старте, затем по расписанию.
public class IngestionSyncWorker(
    IServiceScopeFactory scopeFactory,
    IConfiguration config,
    ILogger<IngestionSyncWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var minutes = config.GetValue<int?>("Ingestion:SyncIntervalMinutes") ?? 15;
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(minutes));

        do
        {
            try
            {
                // SyncService — scoped (свой DbContext/IPublishEndpoint на прогон)
                using var scope = scopeFactory.CreateScope();
                var sync = scope.ServiceProvider.GetRequiredService<SyncService>();
                await sync.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;   // штатная остановка
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sync run failed; will retry next tick");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
