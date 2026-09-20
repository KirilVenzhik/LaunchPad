namespace LaunchPad.BuildingBlocks;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;
}
