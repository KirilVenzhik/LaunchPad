using LaunchPad.BuildingBlocks;
using LaunchPad.Catalog.Domain.Enums;

namespace LaunchPad.Catalog.Domain.Entities;

public class Agency : Entity<long>
{
    public Guid Ll2Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Abbrev { get; private set; }
    public AgencyType Type { get; private set; }
    public string? CountryCode { get; private set; }

    private Agency() { }

    public static Agency Create(Guid ll2Id, string name, string? abbrev, AgencyType type, string? countryCode)
        => new() { Ll2Id = ll2Id, Name = name, Abbrev = abbrev, Type = type, CountryCode = countryCode };
}
