using LaunchPad.BuildingBlocks;

namespace LaunchPad.Catalog.Domain.Entities;

public class Pad : Entity<long>
{
    public int Ll2Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? CountryCode { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    private Pad() { }

    public static Pad Create(int ll2Id, string name, string? countryCode, double? latitude, double? longitude)
        => new() { Ll2Id = ll2Id, Name = name, CountryCode = countryCode, Latitude = latitude, Longitude = longitude };
}
