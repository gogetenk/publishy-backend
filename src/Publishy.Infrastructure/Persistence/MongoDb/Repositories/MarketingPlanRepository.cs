using MongoDB.Driver;
using Publishy.Domain.MarketingPlans;
using Publishy.Domain.Posts;

namespace Publishy.Infrastructure.Persistence.MongoDb.Repositories;

public class MarketingPlanRepository : IMarketingPlanRepository
{
    private readonly IMongoCollection<MarketingPlan> _marketingPlans;

    public MarketingPlanRepository(MongoDbContext context)
    {
        _marketingPlans = context.GetCollection<MarketingPlan>("MarketingPlans");
    }

    public async Task<MarketingPlan?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _marketingPlans.Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MarketingPlan> AddAsync(MarketingPlan plan, CancellationToken cancellationToken = default)
    {
        await _marketingPlans.InsertOneAsync(plan, cancellationToken: cancellationToken);
        return plan;
    }

    public async Task UpdateAsync(MarketingPlan plan, CancellationToken cancellationToken = default)
    {
        await _marketingPlans.ReplaceOneAsync(
            p => p.Id == plan.Id,
            plan,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(string planId, CancellationToken cancellationToken = default)
    {
        var plan = await GetByIdAsync(planId, cancellationToken);
        return plan?.Posts ?? Enumerable.Empty<Post>();
    }
}