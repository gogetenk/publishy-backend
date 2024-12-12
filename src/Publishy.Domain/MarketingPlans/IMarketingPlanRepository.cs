namespace Publishy.Domain.MarketingPlans;

public interface IMarketingPlanRepository
{
    Task<MarketingPlan?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<MarketingPlan> AddAsync(MarketingPlan plan, CancellationToken cancellationToken = default);
    Task UpdateAsync(MarketingPlan plan, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPostsAsync(string planId, CancellationToken cancellationToken = default);
}