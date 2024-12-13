namespace Publishy.Application.Domain.ValueObjects;

public record AnalyticsMetric(
    string Name,
    string Category,
    decimal Value,
    string Unit,
    DateTime Timestamp,
    Dictionary<string, string> Dimensions
);