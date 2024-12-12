using MongoDB.Driver;
using Publishy.Domain.Analytics;
using Publishy.Infrastructure.Persistence.MongoDb;

namespace Publishy.Infrastructure.Analytics;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IMongoCollection<Analytics> _analytics;
    private readonly IMongoCollection<AnalyticsSnapshot> _snapshots;

    public AnalyticsRepository(MongoDbContext context)
    {
        _analytics = context.GetCollection<Analytics>("Analytics");
        _snapshots = context.GetCollection<AnalyticsSnapshot>("AnalyticsSnapshots");
    }

    public async Task<GlobalPerformance> GetGlobalPerformanceAsync(CancellationToken cancellationToken = default)
    {
        var analytics = await _analytics.Find(_ => true)
            .FirstOrDefaultAsync(cancellationToken);

        return analytics?.GlobalPerformance ?? new GlobalPerformance(0, 0);
    }

    public async Task<IEnumerable<NetworkDistribution>> GetNetworkDistributionAsync(CancellationToken cancellationToken = default)
    {
        var analytics = await _analytics.Find(_ => true)
            .FirstOrDefaultAsync(cancellationToken);

        return analytics?.NetworkDistributions ?? Enumerable.Empty<NetworkDistribution>();
    }

    public async Task<int> GetScheduledPostsCountAsync(CancellationToken cancellationToken = default)
    {
        var analytics = await _analytics.Find(_ => true)
            .FirstOrDefaultAsync(cancellationToken);

        return analytics?.ScheduledPostsCount ?? 0;
    }
}