using System.Text.Json;
using LaunchPad.Contracts.Events;
using LaunchPad.Ingestion.Acl;
using LaunchPad.Ingestion.Domain;
using LaunchPad.Ingestion.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace LaunchPad.Ingestion.Sync;

// Оркестрация одного цикла синхронизации:
// тянем LL2 → маппим → хэшируем → сравниваем с sync_state → публикуем изменения.
public class SyncService(
    ILaunchLibraryClient client,
    IngestionDbContext db,
    IPublishEndpoint publish,
    ILogger<SyncService> logger)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        int seen = 0, published = 0, skipped = 0;

        await foreach (var raw in client.GetUpcomingAsync(ct))
        {
            try
            {
                var upserted = Ll2Mapper.ToUpserted(raw, now);
                var hash = ContentHasher.Compute(upserted);

                var state = await db.SyncStates
                    .FirstOrDefaultAsync(x => x.Ll2Id == upserted.LaunchLl2Id, ct);

                if (state is null)
                {
                    db.SyncStates.Add(SyncState.Create(upserted.LaunchLl2Id, hash, Serialize(upserted), now));
                    await publish.Publish(upserted, ct);   // уйдёт в outbox, отправится после SaveChanges
                    published++;
                }
                else if (!state.ContentHash.SequenceEqual(hash))
                {
                    state.Update(hash, Serialize(upserted), now);
                    await publish.Publish(upserted, ct);
                    published++;
                }
                else
                {
                    state.Touch(now);   // без изменений — событие не шлём
                }

                seen++;
            }
            catch (Exception ex)
            {
                skipped++;   // один битый запуск не должен ронять весь батч
                logger.LogWarning(ex, "Skipping launch {LaunchId}", raw.Id);
            }
        }

        await db.SaveChangesAsync(ct);   // атомарно: sync_state + outbox-события
        logger.LogInformation(
            "Sync complete: seen={Seen}, published={Published}, skipped={Skipped}",
            seen, published, skipped);
    }

    private static string Serialize(LaunchUpserted u) => JsonSerializer.Serialize(u);
}
