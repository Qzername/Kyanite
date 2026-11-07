namespace Kyanite.Database;

public record DataInformation
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
}
