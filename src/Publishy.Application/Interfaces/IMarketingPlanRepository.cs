using Publishy.Application.Domain.AggregateRoots;

namespace Publishy.Application.Interfaces;

public interface IMarketingPlanRepository
{
    Task<MarketingPlan?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MarketingPlan>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        MarketingPlanStatus? status = null,
        DateTime? startDateAfter = null,
        DateTime? startDateBefore = null,
        CancellationToken cancellationToken = default);
    Task<MarketingPlan> AddAsync(MarketingPlan marketingPlan, CancellationToken cancellationToken = default);
    Task UpdateAsync(MarketingPlan marketingPlan, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(
        string? projectId = null,
        MarketingPlanStatus? status = null,
        DateTime? startDateAfter = null,
        DateTime? startDateBefore = null,
        CancellationToken cancellationToken = default);
}