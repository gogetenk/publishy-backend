namespace Publishy.Domain.Analytics;

public interface IAnalyticsSnapshotRepository
{
    Task<AnalyticsSnapshot?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AnalyticsSnapshot> AddAsync(AnalyticsSnapshot snapshot, CancellationToken cancellationToken = default);
    Task<IEnumerable<AnalyticsSnapshot>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}