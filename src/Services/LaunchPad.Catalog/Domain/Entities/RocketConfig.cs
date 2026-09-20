using LaunchPad.BuildingBlocks;

namespace LaunchPad.Catalog.Domain.Entities;

public class RocketConfig : Entity<long>
{
    public Guid Ll2Id { get; private set; }
    public string FullName { get; private set; } = null!;
    public string? Family { get; private set; }
    public int ReusableStages { get; private set; }

    private RocketConfig() { }

    public static RocketConfig Create(Guid ll2Id, string fullName, string? family, int reusableStages)
        => new() { Ll2Id = ll2Id, FullName = fullName, Family = family, ReusableStages = reusableStages };
}
