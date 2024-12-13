using Publishy.Application.Domain.AggregateRoots;

namespace Publishy.Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<Analytics?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Analytics>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        string? source = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
    Task<Analytics> AddAsync(Analytics analytics, CancellationToken cancellationToken = default);
    Task UpdateAsync(Analytics analytics, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(
        string? projectId = null,
        string? source = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}