namespace LaunchPad.Contracts.Events;

public record LaunchUpserted
{
    public required Guid LaunchLl2Id { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset NetUtc { get; init; }
    public DateTimeOffset? WindowStartUtc { get; init; }
    public DateTimeOffset? WindowEndUtc { get; init; }

    public required AgencyPayload Agency { get; init; }
    public required RocketConfigPayload Rocket { get; init; }
    public required PadPayload Pad { get; init; }

    public string? MissionName { get; init; }
    public string? Orbit { get; init; }

    public DateTimeOffset OccurredUtc { get; init; }
}

public record AgencyPayload
{
    public required int Ll2Id { get; init; }
    public required string Name { get; init; }
    public string? Abbrev { get; init; }
    public string? Type { get; init; }
    public string? CountryCode { get; init; }
}

public record RocketConfigPayload
{
    public required int Ll2Id { get; init; }
    public required string FullName { get; init; }
    public string? Family { get; init; }
    public bool Reusable { get; init; }
}

public record PadPayload
{
    public required int Ll2Id { get; init; }
    public required string Name { get; init; }
    public string? CountryCode { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}
