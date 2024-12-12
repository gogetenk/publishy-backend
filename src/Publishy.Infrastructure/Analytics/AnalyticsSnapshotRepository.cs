using MongoDB.Driver;
using Publishy.Domain.Analytics;
using Publishy.Infrastructure.Persistence.MongoDb;

namespace Publishy.Infrastructure.Analytics;

public class AnalyticsSnapshotRepository : IAnalyticsSnapshotRepository
{
    private readonly IMongoCollection<AnalyticsSnapshot> _snapshots;

    public AnalyticsSnapshotRepository(MongoDbContext context)
    {
        _snapshots = context.GetCollection<AnalyticsSnapshot>("AnalyticsSnapshots");
    }

    public async Task<AnalyticsSnapshot?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _snapshots.Find(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AnalyticsSnapshot> AddAsync(AnalyticsSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _snapshots.InsertOneAsync(snapshot, cancellationToken: cancellationToken);
        return snapshot;
    }

    public async Task<IEnumerable<AnalyticsSnapshot>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _snapshots.Find(s => s.SnapshotDate >= start && s.SnapshotDate <= end)
            .ToListAsync(cancellationToken);
    }
}