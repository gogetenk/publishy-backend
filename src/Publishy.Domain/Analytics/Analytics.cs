using Ardalis.Result;
using Publishy.Domain.Posts;

namespace Publishy.Domain.Analytics;

public class Analytics
{
    public string Id { get; private set; }
    public DateTime Date { get; private set; }
    public GlobalPerformance GlobalPerformance { get; private set; }
    public List<NetworkDistribution> NetworkDistributions { get; private set; }
    public int ScheduledPostsCount { get; private set; }

    private Analytics() { } // For EF Core

    private Analytics(GlobalPerformance globalPerformance, List<NetworkDistribution> networkDistributions, int scheduledPostsCount)
    {
        Id = Guid.NewGuid().ToString();
        Date = DateTime.UtcNow;
        GlobalPerformance = globalPerformance;
        NetworkDistributions = networkDistributions;
        ScheduledPostsCount = scheduledPostsCount;
    }

    public static Result<Analytics> Create(GlobalPerformance globalPerformance, List<NetworkDistribution> networkDistributions, int scheduledPostsCount)
    {
        if (globalPerformance == null)
            return Result.Error("Global performance metrics are required");

        if (networkDistributions == null || !networkDistributions.Any())
            return Result.Error("Network distribution metrics are required");

        if (scheduledPostsCount < 0)
            return Result.Error("Scheduled posts count cannot be negative");

        return Result.Success(new Analytics(globalPerformance, networkDistributions, scheduledPostsCount));
    }

    public Result UpdateGlobalPerformance(int totalPublishedPosts, int totalProjects)
    {
        if (totalPublishedPosts < 0)
            return Result.Error("Total published posts cannot be negative");

        if (totalProjects < 0)
            return Result.Error("Total projects cannot be negative");

        GlobalPerformance = new GlobalPerformance(totalPublishedPosts, totalProjects);
        return Result.Success();
    }

    public Result UpdateNetworkDistribution(string network, MediaTypePercentages percentages)
    {
        if (string.IsNullOrWhiteSpace(network))
            return Result.Error("Network name cannot be empty");

        if (percentages == null)
            return Result.Error("Media type percentages are required");

        if (!IsValidPercentages(percentages))
            return Result.Error("Percentages must sum up to 100%");

        var existingDistribution = NetworkDistributions.FirstOrDefault(d => d.Network == network);
        if (existingDistribution != null)
        {
            NetworkDistributions.Remove(existingDistribution);
        }

        NetworkDistributions.Add(new NetworkDistribution(network, percentages));
        return Result.Success();
    }

    public Result UpdateScheduledPostsCount(int count)
    {
        if (count < 0)
            return Result.Error("Scheduled posts count cannot be negative");

        ScheduledPostsCount = count;
        return Result.Success();
    }

    private static bool IsValidPercentages(MediaTypePercentages percentages)
    {
        const float tolerance = 0.01f;
        var sum = percentages.Text + percentages.Image + percentages.Video;
        return Math.Abs(sum - 100.0f) < tolerance;
    }
}