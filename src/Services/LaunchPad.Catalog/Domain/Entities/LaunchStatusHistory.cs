using LaunchPad.BuildingBlocks;
using LaunchPad.Catalog.Domain.Enums;

namespace LaunchPad.Catalog.Domain.Entities;

public class LaunchStatusHistory : Entity<long>
{
    public LaunchStatusHistory(
        ChangeType type,
        DateTimeOffset? oldNetUtc,
        DateTimeOffset? newNetUtc,
        LaunchStatus? oldStatus,
        LaunchStatus? newStatus,
        DateTimeOffset detectedUtc)
    {
        ChangeType = type;
        OldNetUtc = oldNetUtc;
        NewNetUtc = newNetUtc;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        DetectedUtc = detectedUtc;
    }

    public long LaunchId { get; private set; }
    public ChangeType ChangeType { get; private set; }
    public DateTimeOffset? OldNetUtc { get; private set; }
    public DateTimeOffset? NewNetUtc { get; private set; }
    public LaunchStatus? OldStatus { get; private set; }
    public LaunchStatus? NewStatus { get; private set; }
    public DateTimeOffset DetectedUtc { get; private set; }

    private LaunchStatusHistory() { }
}
