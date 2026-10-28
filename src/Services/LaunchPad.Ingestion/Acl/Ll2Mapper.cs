using LaunchPad.Contracts.Events;
using LaunchPad.Ingestion.Acl.DTOs;
using System.Globalization;

namespace LaunchPad.Ingestion.Acl;

public static class Ll2Mapper
{
    public static LaunchUpserted ToUpserted(Ll2Launch l, DateTimeOffset occurredUtc)
    {
        var provider = l.LaunchServiceProvider
            ?? throw new InvalidOperationException($"Launch {l.Id}: no launch_service_provider");
        var config = l.Rocket?.Configuration
            ?? throw new InvalidOperationException($"Launch {l.Id}: no rocket.configuration");
        var pad = l.Pad
            ?? throw new InvalidOperationException($"Launch {l.Id}: no pad");

        return new LaunchUpserted
        {
            LaunchLl2Id = l.Id,
            Name = l.Name,
            Status = MapStatus(l.Status?.Abbrev),
            NetUtc = l.Net,
            WindowStartUtc = l.WindowStart,
            WindowEndUtc = l.WindowEnd,
            Agency = new AgencyPayload
            {
                Ll2Id = provider.Id,
                Name = provider.Name ?? "Unknown",
                Abbrev = provider.Abbrev,
                Type = provider.Type,
                CountryCode = provider.CountryCode
            },
            Rocket = new RocketConfigPayload
            {
                Ll2Id = config.Id,
                FullName = config.FullName ?? config.Name ?? "Unknown",
                Family = config.Family,
                Reusable = config.Reusable ?? false
            },
            Pad = new PadPayload
            {
                Ll2Id = pad.Id,
                Name = pad.Name ?? "Unknown",
                CountryCode = pad.CountryCode,
                Latitude = ParseDouble(pad.Latitude),
                Longitude = ParseDouble(pad.Longitude)
            },
            MissionName = l.Mission?.Name,
            Orbit = l.Mission?.Orbit?.Abbrev,
            OccurredUtc = occurredUtc
        };
    }

    private static string MapStatus(string? abbrev) => abbrev switch
    {
        "Go" => "Go",
        "TBD" => "Tbd",
        "TBC" => "Tbd",
        "Hold" => "Hold",
        "In Flight" => "InFlight",
        "Success" => "Success",
        "Failure" => "Failure",
        "Partial Failure" => "Failure",
        _ => "Unknown"
    };

    private static double? ParseDouble(string? s)
        => double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : null;
}
