using Publishy.Domain.Posts;

namespace Publishy.Domain.Analytics;

public interface IAnalyticsRepository
{
    Task<GlobalPerformance> GetGlobalPerformanceAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NetworkDistribution>> GetNetworkDistributionAsync(CancellationToken cancellationToken = default);
    Task<int> GetScheduledPostsCountAsync(CancellationToken cancellationToken = default);
}