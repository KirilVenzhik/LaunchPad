namespace LaunchPad.Ingestion.Acl.DTOs;

public sealed class Ll2Launch
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public Ll2Status? Status { get; set; }
    public DateTimeOffset Net { get; set; }
    public DateTimeOffset? WindowStart { get; set; }
    public DateTimeOffset? WindowEnd { get; set; }
    public Ll2Provider? LaunchServiceProvider { get; set; }
    public Ll2Rocket? Rocket { get; set; }
    public Ll2Pad? Pad { get; set; }
    public Ll2Mission? Mission { get; set; }
}