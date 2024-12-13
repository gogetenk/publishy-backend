namespace Publishy.Application.Domain.ValueObjects;

public record TargetAudience
{
    public string Type { get; init; }
    public string Description { get; init; }

    public TargetAudience(string type, string description)
    {
        Type = type;
        Description = description;
    }
}