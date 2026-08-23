namespace Kyanite.Database;

public record ModuleInformation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
}