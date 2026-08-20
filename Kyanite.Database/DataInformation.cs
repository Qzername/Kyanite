namespace Kyanite.Database;

public record DataInformation
{
    public string Id { get; set; }
    public string Type { get; set; }
    public object Value { get; set; }
}
