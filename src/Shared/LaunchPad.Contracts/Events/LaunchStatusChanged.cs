namespace LaunchPad.Contracts.Events;

public record LaunchStatusChanged
{
    public required Guid LaunchLl2Id { get; init; }
    public required string LaunchName { get; init; }
    public required string OldStatus { get; init; }
    public required string NewStatus { get; init; }
    public DateTimeOffset OccurredUtc { get; init; }
}
