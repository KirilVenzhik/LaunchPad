using LaunchPad.Contracts.Events;
using LaunchPad.Ingestion.Domain;

namespace LaunchPad.Ingestion.UnitTests.Domain;

public class ContentHasherTests
{
    private static readonly Guid LaunchId = Guid.Parse("f059f1a4-7d4e-4a5b-9c2e-3b1d2a6e8c01");
    private static readonly DateTimeOffset Net = new(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset OccurredUtc = new(2026, 10, 26, 9, 0, 0, TimeSpan.Zero);
    private static LaunchUpserted ValidLaunch()
    {
        return new LaunchUpserted
        {
            LaunchLl2Id = LaunchId,
            Name = "Falcon 9 Block 5 | Starlink Group 12-7",
            Status = "Go",
            NetUtc = Net,
            WindowStartUtc = Net.AddMinutes(-30),
            WindowEndUtc = Net.AddHours(2),
            Agency = new AgencyPayload
            {
                Ll2Id = 121,
                Name = "SpaceX",
                Abbrev = "SpX",
                Type = "Commercial",
                CountryCode = "USA"
            },
            Rocket = new RocketConfigPayload
            {
                Ll2Id = 164,
                FullName = "Falcon 9 Block 5",
                Family = "Falcon",
                Reusable = true
            },
            Pad = new PadPayload
            {
                Ll2Id = 87,
                Name = "Launch Complex 39A",
                CountryCode = "USA",
                Latitude = 28.60822681,
                Longitude = -80.60428186
            },
            MissionName = "Starlink Group 12-7",
            Orbit = "LEO",
            OccurredUtc = OccurredUtc
        };
    }

    [Fact]
    public void Compute_SameLaunch_ReturnsSameHash()
    {
        var firstLaunch = ValidLaunch();
        var secondLaunch = ValidLaunch();

        var firstLaunchHash = ContentHasher.Compute(firstLaunch);
        var secondLaunchHash = ContentHasher.Compute(secondLaunch);

        Assert.Equal(firstLaunchHash, secondLaunchHash);
    }

    [Fact]
    public void Compute_OccurredUtcChanged_ReturnsSameHash()
    {
        var firstLaunch = ValidLaunch();
        var secondLaunch = ValidLaunch() with { OccurredUtc = OccurredUtc.AddHours(2) };

        var firstLaunchHash = ContentHasher.Compute(firstLaunch);
        var secondLaunchHash = ContentHasher.Compute(secondLaunch);

        Assert.Equal(firstLaunchHash, secondLaunchHash);
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Status")]
    [InlineData("NetUtc")]
    [InlineData("WindowStartUtc")]
    [InlineData("WindowEndUtc")]
    [InlineData("Agency.Ll2Id")]
    [InlineData("Rocket.Ll2Id")]
    [InlineData("Pad.Ll2Id")]
    [InlineData("Orbit")]
    [InlineData("MissionName")]
    public void Compute_SignificantFieldChanged_ReturnsDifferentHash(string field)
    {
        var baseline = ValidLaunch();
        var changed = field switch
        {
            "Name" => baseline with { Name = "Falcon 9 Block 5 | Starlink Group 12-8" },
            "Status" => baseline with { Status = "Hold" },
            "NetUtc" => baseline with { NetUtc = Net.AddDays(1) },
            "WindowStartUtc" => baseline with { WindowStartUtc = null },
            "WindowEndUtc" => baseline with { WindowEndUtc = null },
            "Agency.Ll2Id" => baseline with { Agency = baseline.Agency with { Ll2Id = 999 } },
            "Rocket.Ll2Id" => baseline with { Rocket = baseline.Rocket with { Ll2Id = 999 } },
            "Pad.Ll2Id" => baseline with { Pad = baseline.Pad with { Ll2Id = 999 } },
            "Orbit" => baseline with { Orbit = "GTO" },
            "MissionName" => baseline with { MissionName = "Starlink Group 12-8" },
            _ => throw new ArgumentException($"Unknown field: {field}", nameof(field))
        };

        Assert.NotEqual(ContentHasher.Compute(baseline), ContentHasher.Compute(changed));
    }
}