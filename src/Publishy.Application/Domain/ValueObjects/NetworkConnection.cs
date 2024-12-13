namespace Publishy.Application.Domain.ValueObjects;

public record NetworkConnection(
    string SourceId,
    string TargetId,
    string Type,
    decimal Strength,
    Dictionary<string, string> Metadata
);