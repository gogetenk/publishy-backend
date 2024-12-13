using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Commands.CreateAnalytics;

public record AnalyticsResponse(
    string Id,
    string ProjectId,
    string Source,
    AnalyticsPeriod Period,
    List<AnalyticsMetric> Metrics,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
)
{
    public static explicit operator AnalyticsResponse(Domain.AggregateRoots.Analytics analytics)
    {
        return new AnalyticsResponse(
            analytics.Id,
            analytics.ProjectId,
            analytics.Source,
            analytics.Period,
            analytics.Metrics,
            analytics.CreatedAt,
            analytics.LastUpdatedAt
        );
    }
}