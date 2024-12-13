namespace Publishy.Application.Domain.ValueObjects;

public record NetworkMetrics(
    int TotalNodes,
    int TotalConnections,
    decimal AverageConnectionStrength,
    decimal Density,
    Dictionary<string, decimal> CustomMetrics
);