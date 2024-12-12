using Ardalis.Result;

namespace Publishy.Domain.Analytics;

public class AnalyticsSnapshot
{
    public string Id { get; private set; }
    public DateTime SnapshotDate { get; private set; }
    public GlobalPerformance GlobalPerformance { get; private set; }
    public List<NetworkDistribution> NetworkDistributions { get; private set; }
    public int ScheduledPostsCount { get; private set; }

    private AnalyticsSnapshot() { } // For EF Core

    private AnalyticsSnapshot(Analytics analytics)
    {
        Id = Guid.NewGuid().ToString();
        SnapshotDate = DateTime.UtcNow;
        GlobalPerformance = analytics.GlobalPerformance;
        NetworkDistributions = analytics.NetworkDistributions.ToList();
        ScheduledPostsCount = analytics.ScheduledPostsCount;
    }

    public static Result<AnalyticsSnapshot> Create(Analytics analytics)
    {
        if (analytics == null)
            return Result.Error("Analytics data is required to create a snapshot");

        return Result.Success(new AnalyticsSnapshot(analytics));
    }
}