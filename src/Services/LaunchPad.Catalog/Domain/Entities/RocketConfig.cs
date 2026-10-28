using LaunchPad.BuildingBlocks;

namespace LaunchPad.Catalog.Domain.Entities;

public class RocketConfig : Entity<long>
{
    public int Ll2Id { get; private set; }
    public string FullName { get; private set; } = null!;
    public string? Family { get; private set; }
    public bool Reusable { get; private set; }

    private RocketConfig() { }

    public static RocketConfig Create(int ll2Id, string fullName, string? family, bool reusable)
        => new() { Ll2Id = ll2Id, FullName = fullName, Family = family, Reusable = reusable };
}
