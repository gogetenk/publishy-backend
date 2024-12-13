using Ardalis.Result;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.Domain.AggregateRoots;

public class Analytics
{
    public string Id { get; private set; }
    public string ProjectId { get; private set; }
    public string Source { get; private set; }
    public AnalyticsPeriod Period { get; private set; }
    public List<AnalyticsMetric> Metrics { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private Analytics() { } // For MongoDB

    private Analytics(
        string projectId,
        string source,
        AnalyticsPeriod period,
        List<AnalyticsMetric> metrics)
    {
        Id = Guid.NewGuid().ToString();
        ProjectId = projectId;
        Source = source;
        Period = period;
        Metrics = metrics;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public static Result<Analytics> Create(
        string projectId,
        string source,
        AnalyticsPeriod period,
        List<AnalyticsMetric> metrics)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return Result.Error("Project ID cannot be empty");

        if (string.IsNullOrWhiteSpace(source))
            return Result.Error("Source cannot be empty");

        if (period.StartDate >= period.EndDate)
            return Result.Error("Start date must be before end date");

        if (!metrics.Any())
            return Result.Error("At least one metric must be provided");

        if (metrics.Any(m => m.Timestamp < period.StartDate || m.Timestamp > period.EndDate))
            return Result.Error("All metrics must be within the specified period");

        return Result.Success(new Analytics(projectId, source, period, metrics));
    }

    public Result AddMetric(AnalyticsMetric metric)
    {
        if (metric.Timestamp < Period.StartDate || metric.Timestamp > Period.EndDate)
            return Result.Error("Metric timestamp must be within the analytics period");

        Metrics.Add(metric);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result AddMetrics(List<AnalyticsMetric> metrics)
    {
        if (metrics.Any(m => m.Timestamp < Period.StartDate || m.Timestamp > Period.EndDate))
            return Result.Error("All metrics must be within the analytics period");

        Metrics.AddRange(metrics);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdatePeriod(AnalyticsPeriod newPeriod)
    {
        if (newPeriod.StartDate >= newPeriod.EndDate)
            return Result.Error("Start date must be before end date");

        if (Metrics.Any(m => m.Timestamp < newPeriod.StartDate || m.Timestamp > newPeriod.EndDate))
            return Result.Error("Cannot update period: some existing metrics would be outside the new period");

        Period = newPeriod;
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}