namespace Publishy.Application.Domain.ValueObjects;

public record MarketingGoal(
    string Name,
    string Description,
    string MetricType,
    decimal TargetValue,
    DateTime TargetDate
);