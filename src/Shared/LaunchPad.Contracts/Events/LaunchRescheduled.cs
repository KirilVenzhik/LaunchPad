namespace LaunchPad.Contracts.Events;

public record LaunchRescheduled
{
    public required Guid LaunchLl2Id { get; init; }
    public required string LaunchName { get; init; }
    public required DateTimeOffset OldNetUtc { get; init; }
    public required DateTimeOffset NewNetUtc { get; init; }
    public DateTimeOffset OccurredUtc { get; init; }
}
