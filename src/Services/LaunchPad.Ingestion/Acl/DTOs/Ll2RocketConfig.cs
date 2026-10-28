namespace LaunchPad.Ingestion.Acl.DTOs;

public sealed class Ll2RocketConfig 
{ 
    public int Id { get; set; } 
    public string? Name { get; set; } 
    public string? Family { get; set; } 
    public string? FullName { get; set; } 
    public bool? Reusable { get; set; } 
}