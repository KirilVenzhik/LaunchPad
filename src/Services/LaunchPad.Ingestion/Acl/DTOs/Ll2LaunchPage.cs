namespace LaunchPad.Ingestion.Acl.DTOs;

public sealed class Ll2LaunchPage
{
    public int Count { get; set; }
    public string? Next { get; set; }
    public List<Ll2Launch> Results { get; set; } = [];
}
