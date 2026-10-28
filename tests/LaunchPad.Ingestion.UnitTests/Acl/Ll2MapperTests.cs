using LaunchPad.Contracts.Events;
using LaunchPad.Ingestion.Acl;
using LaunchPad.Ingestion.Acl.DTOs;

namespace LaunchPad.Ingestion.UnitTests.Acl;

public class Ll2MapperTests
{
    private static readonly Guid LaunchId = Guid.Parse("f059f1a4-7d4e-4a5b-9c2e-3b1d2a6e8c01");
    private static readonly DateTimeOffset Net = new(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset OccurredUtc = new(2026, 10, 26, 9, 0, 0, TimeSpan.Zero);
    private static Ll2Launch ValidLaunch()
    {
        return new Ll2Launch
        {
            Id = LaunchId,
            Name = "Falcon 9 Block 5 | Starlink Group 12-7",
            Status = new Ll2Status
            {
                Abbrev = "Go",
                Name = "Go for Launch"
            },
            Net = Net,
            WindowStart = Net.AddMinutes(-30),
            WindowEnd = Net.AddHours(2),
            LaunchServiceProvider = new Ll2Provider
            {
                Id = 121,
                Name = "SpaceX",
                Abbrev = "SpX",
                Type = "Commercial",
                CountryCode = "USA"
            },
            Rocket = new Ll2Rocket
            {
                Configuration = new Ll2RocketConfig
                {
                    Id = 164,
                    Name = "Falcon 9",
                    FullName = "Falcon 9 Block 5",
                    Family = "Falcon",
                    Reusable = true
                }
            },
            Pad = new Ll2Pad
            {
                Id = 87,
                Name = "Launch Complex 39A",
                CountryCode = "USA",
                Latitude = "28.60822681",
                Longitude = "-80.60428186"
            },
            Mission = new Ll2Mission
            {
                Name = "Starlink Group 12-7",
                Orbit = new Ll2Orbit
                {
                    Abbrev = "LEO",
                    Name = "Low Earth Orbit"
                }
            }
        };
    }

    [Fact]
    public void ToUpserted_LaunchServiceProviderIsNull_ThrowsInvalidOperationException()
    {
        var launch = ValidLaunch();

        launch.LaunchServiceProvider = null;

        var ex = Assert.Throws<InvalidOperationException>(() => Ll2Mapper.ToUpserted(launch, OccurredUtc));
        Assert.Contains("no launch_service_provider", ex.Message);
    }

    [Fact]
    public void ToUpserted_RocketIsNull_ThrowsInvalidOperationException()
    {
        var launch = ValidLaunch();

        launch.Rocket = null;

        var ex = Assert.Throws<InvalidOperationException>(() => Ll2Mapper.ToUpserted(launch, OccurredUtc));
        Assert.Contains("no rocket.configuration", ex.Message);
    }

    [Fact]
    public void ToUpserted_RocketConfigurationIsNull_ThrowsInvalidOperationException()
    {
        var launch = ValidLaunch();

        launch.Rocket!.Configuration = null;

        var ex = Assert.Throws<InvalidOperationException>(() => Ll2Mapper.ToUpserted(launch, OccurredUtc));
        Assert.Contains("no rocket.configuration", ex.Message);
    }

    [Fact]
    public void ToUpserted_PadIsNull_ThrowsInvalidOperationException()
    {
        var launch = ValidLaunch();

        launch.Pad = null;

        var ex = Assert.Throws<InvalidOperationException>(() => Ll2Mapper.ToUpserted(launch, OccurredUtc));
        Assert.Contains("no pad", ex.Message);
    }

    [Fact]
    public void ToUpserted_ValidLaunch_MapsAllFields()
    {
        var launch = ValidLaunch();
        var expected = new LaunchUpserted
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

        var actual = Ll2Mapper.ToUpserted(launch, OccurredUtc);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToUpserted_MissionIsNull_MissionNameAndOrbitAreNull()
    {
        var launch = ValidLaunch();

        launch.Mission = null;
        var newLaunch = Ll2Mapper.ToUpserted(launch, OccurredUtc);

        Assert.Null(newLaunch.MissionName);
        Assert.Null(newLaunch.Orbit);
    }

    [Theory]
    [InlineData("Go", "Go")]
    [InlineData("TBD", "Tbd")]
    [InlineData("TBC", "Tbd")]
    [InlineData("Hold", "Hold")]
    [InlineData("In Flight", "InFlight")]
    [InlineData("Success", "Success")]
    [InlineData("Failure", "Failure")]
    [InlineData("Partial Failure", "Failure")]
    [InlineData("RandomAbbrev", "Unknown")]
    public void ToUpserted_StatusAbbrev_MapsToContractStatus(string statusAbbrev, string expectedStatus)
    {
        var launch = ValidLaunch();

        launch.Status!.Abbrev = statusAbbrev;

        Assert.Equal(expectedStatus, Ll2Mapper.ToUpserted(launch, OccurredUtc).Status);
    }

    [Fact]
    public void ToUpserted_StatusIsNull_StatusIsUnknown()
    {
        var launch = ValidLaunch();

        launch.Status = null;

        Assert.Equal("Unknown", Ll2Mapper.ToUpserted(launch, OccurredUtc).Status);
    }

    [Fact]
    public void ToUpserted_ProviderNameIsNull_AgencyNameIsUnknown()
    {
        var launch = ValidLaunch();

        launch.LaunchServiceProvider!.Name = null;

        Assert.Equal("Unknown", Ll2Mapper.ToUpserted(launch, OccurredUtc).Agency.Name);
    }

    [Fact]
    public void ToUpserted_RocketFullNameIsNull_FullNameFallsBackToName()
    {
        var launch = ValidLaunch();

        launch.Rocket!.Configuration!.FullName = null;
        var expected = launch.Rocket!.Configuration!.Name;

        Assert.Equal(expected, Ll2Mapper.ToUpserted(launch, OccurredUtc).Rocket.FullName);
    }

    [Fact]
    public void ToUpserted_RocketFullNameAndNameAreNull_FullNameIsUnknown()
    {
        var launch = ValidLaunch();

        launch.Rocket!.Configuration!.FullName = null;
        launch.Rocket!.Configuration!.Name = null;

        Assert.Equal("Unknown", Ll2Mapper.ToUpserted(launch, OccurredUtc).Rocket.FullName);
    }

    [Fact]
    public void ToUpserted_ReusableIsNull_ReusableIsFalse()
    {
        var launch = ValidLaunch();

        launch.Rocket!.Configuration!.Reusable = null;

        Assert.False(Ll2Mapper.ToUpserted(launch, OccurredUtc).Rocket.Reusable);
    }

    [Fact]
    public void ToUpserted_PadNameIsNull_PadNameIsUnknown()
    {
        var launch = ValidLaunch();

        launch.Pad!.Name = null;

        Assert.Equal("Unknown", Ll2Mapper.ToUpserted(launch, OccurredUtc).Pad.Name);
    }

    [Theory]
    [InlineData("28.5", 28.5)]
    [InlineData("-80.6", -80.6)]
    public void ToUpserted_ValidCoordinates_ParsesToDouble(string latitude, double expected)
    {
        var launch = ValidLaunch();

        launch.Pad!.Latitude = latitude;

        Assert.Equal(expected, Ll2Mapper.ToUpserted(launch, OccurredUtc).Pad.Latitude);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("28,5")]
    public void ToUpserted_InvalidCoordinates_ReturnsNull(string? latitude)
    {
        var launch = ValidLaunch();

        launch.Pad!.Latitude = latitude;

        Assert.Null(Ll2Mapper.ToUpserted(launch, OccurredUtc).Pad.Latitude);
    }
}
