using LaunchPad.Catalog.Domain.Enums;

namespace LaunchPad.Catalog.Application;

public static class EventMapping
{
    public static LaunchStatus ToStatus(string? s)
        => Enum.TryParse<LaunchStatus>(s, true, out var v) ? v : LaunchStatus.Unknown;
    public static Orbit ToOrbit(string? s)
        => Enum.TryParse<Orbit>(s, true, out var v) ? v : Orbit.Unknown;
    public static AgencyType ToAgencyType(string? s)
        => Enum.TryParse<AgencyType>(s, true, out var v) ? v : AgencyType.Unknown;
}
