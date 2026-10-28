namespace LaunchPad.Ingestion.Domain;

// Состояние синхронизации одного запуска: хэш значимых полей для change-detection.
// PK — LL2 UUID запуска. Canonical-данные живут в Catalog, здесь только след синка.
public class SyncState
{
    public Guid Ll2Id { get; private set; }
    public byte[] ContentHash { get; private set; } = [];
    public string LastSnapshotJson { get; private set; } = "{}"; // jsonb, для дельт/отладки
    public DateTimeOffset FirstSeenUtc { get; private set; }
    public DateTimeOffset LastSyncedUtc { get; private set; }

    private SyncState() { }

    public static SyncState Create(Guid ll2Id, byte[] hash, string snapshotJson, DateTimeOffset nowUtc)
        => new()
        {
            Ll2Id = ll2Id,
            ContentHash = hash,
            LastSnapshotJson = snapshotJson,
            FirstSeenUtc = nowUtc,
            LastSyncedUtc = nowUtc
        };

    // Значимые поля изменились — обновляем хэш и снапшот.
    public void Update(byte[] hash, string snapshotJson, DateTimeOffset nowUtc)
    {
        ContentHash = hash;
        LastSnapshotJson = snapshotJson;
        LastSyncedUtc = nowUtc;
    }

    // Ничего не изменилось — только отметка, что видели в этом синке.
    public void Touch(DateTimeOffset nowUtc) => LastSyncedUtc = nowUtc;
}
