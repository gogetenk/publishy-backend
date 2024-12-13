using MongoDB.Driver;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.MongoDb;

public class MarketingPlanRepository : IMarketingPlanRepository
{
    private readonly IMongoCollection<MarketingPlan> _marketingPlans;

    public MarketingPlanRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("publishy-db");
        _marketingPlans = database.GetCollection<MarketingPlan>("MarketingPlans");
    }

    public async Task<MarketingPlan?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _marketingPlans.Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<MarketingPlan>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        string? status = null,
        DateTime? startDateAfter = null,
        DateTime? startDateBefore = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<MarketingPlan>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(p => p.ProjectId, projectId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(p => p.Status.ToString(), status);
        }

        if (startDateAfter.HasValue)
        {
            filter &= builder.Gte(p => p.StartDate, startDateAfter.Value);
        }

        if (startDateBefore.HasValue)
        {
            filter &= builder.Lte(p => p.StartDate, startDateBefore.Value);
        }

        return await _marketingPlans.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<MarketingPlan> AddAsync(MarketingPlan marketingPlan, CancellationToken cancellationToken = default)
    {
        await _marketingPlans.InsertOneAsync(marketingPlan, cancellationToken: cancellationToken);
        return marketingPlan;
    }

    public async Task UpdateAsync(MarketingPlan marketingPlan, CancellationToken cancellationToken = default)
    {
        await _marketingPlans.ReplaceOneAsync(
            p => p.Id == marketingPlan.Id,
            marketingPlan,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _marketingPlans.DeleteOneAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? projectId = null,
        string? status = null,
        DateTime? startDateAfter = null,
        DateTime? startDateBefore = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<MarketingPlan>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(p => p.ProjectId, projectId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(p => p.Status.ToString(), status);
        }

        if (startDateAfter.HasValue)
        {
            filter &= builder.Gte(p => p.StartDate, startDateAfter.Value);
        }

        if (startDateBefore.HasValue)
        {
            filter &= builder.Lte(p => p.StartDate, startDateBefore.Value);
        }

        return (int)await _marketingPlans.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}