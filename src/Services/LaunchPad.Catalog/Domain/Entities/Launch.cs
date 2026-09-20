using LaunchPad.BuildingBlocks;
using LaunchPad.Catalog.Domain.Enums;

namespace LaunchPad.Catalog.Domain.Entities;

public class Launch : AggregateRoot<long>
{
    public Guid Ll2Id { get; private set; }
    public string Name { get; private set; } = null!;
    public LaunchStatus Status { get; private set; }

    public DateTimeOffset NetUtc { get; private set; }
    public DateTimeOffset? WindowStartUtc { get; private set; }
    public DateTimeOffset? WindowEndUtc { get; private set; }

    public long AgencyId { get; private set; }
    public Agency Agency { get; private set; } = null!;
    public long RocketConfigId { get; private set; }
    public RocketConfig RocketConfig { get; private set; } = null!;
    public long PadId { get; private set; }
    public Pad Pad { get; private set; } = null!;

    public string? MissionName { get; private set; }
    public Orbit MissionOrbit { get; private set; }

    public DateTimeOffset LastSyncedUtc { get; private set; }

    private readonly List<LaunchStatusHistory> _history = new();
    public IReadOnlyCollection<LaunchStatusHistory> History => _history.AsReadOnly();

    private Launch() { }

    public static Launch Create(
        Guid ll2Id,
        string name,
        LaunchStatus status,
        DateTimeOffset netUtc,
        DateTimeOffset? windowStartUtc,
        DateTimeOffset? windowEndUtc,
        Agency agency,
        RocketConfig rocket,
        Pad pad, string? missionName,
        Orbit orbit,
        DateTimeOffset nowUtc)
    {
        var l = new Launch
        {
            Ll2Id = ll2Id,
            Name = name,
            Status = status,
            NetUtc = netUtc,
            WindowStartUtc = windowStartUtc,
            WindowEndUtc = windowEndUtc,
            Agency = agency,
            RocketConfig = rocket,
            Pad = pad,
            MissionName = missionName,
            MissionOrbit = orbit,
            LastSyncedUtc = nowUtc
        };
        l.AddHistory(ChangeType.New, null, netUtc, null, status, nowUtc);
        return l;
    }

    public void UpdateDetails(
        string name,
        DateTimeOffset? windowStartUtc,
        DateTimeOffset? windowEndUtc,
        string? missionName,
        Orbit orbit,
        DateTimeOffset nowUtc)
    {
        Name = name;
        WindowStartUtc = windowStartUtc;
        WindowEndUtc = windowEndUtc;
        MissionName = missionName;
        MissionOrbit = orbit;
        LastSyncedUtc = nowUtc;
    }

    public void Reschedule(
        DateTimeOffset newNet,
        DateTimeOffset nowUtc)
    {
        if (newNet == NetUtc) return;
        var old = NetUtc;
        NetUtc = newNet;
        LastSyncedUtc = nowUtc;
        AddHistory(ChangeType.NetChanged, old, newNet, Status, Status, nowUtc);
    }

    public void ChangeStatus(
        LaunchStatus newStatus,
        DateTimeOffset nowUtc)
    {
        if (newStatus == Status) return;
        var old = Status;
        Status = newStatus;
        LastSyncedUtc = nowUtc;
        var type = newStatus == LaunchStatus.Scrubbed
            ? ChangeType.Scrubbed
            : ChangeType.StatusChanged;
        AddHistory(type, NetUtc, NetUtc, old, newStatus, nowUtc);
    }

    private void AddHistory(
        ChangeType type,
        DateTimeOffset? oldNet,
        DateTimeOffset? newNet,
        LaunchStatus? oldStatus,
        LaunchStatus? newStatus,
        DateTimeOffset nowUtc)
    => _history.Add(new LaunchStatusHistory(type, oldNet, newNet, oldStatus, newStatus, nowUtc));
}
