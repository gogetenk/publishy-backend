using MongoDB.Driver;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.MongoDb;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly IMongoCollection<Analytics> _analytics;

    public AnalyticsRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("publishy-db");
        _analytics = database.GetCollection<Analytics>("Analytics");
    }

    public async Task<Analytics?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _analytics.Find(a => a.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Analytics>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        string? source = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Analytics>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(a => a.ProjectId, projectId);
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            filter &= builder.Eq(a => a.Source, source);
        }

        if (startDate.HasValue)
        {
            filter &= builder.Gte(a => a.Period.StartDate, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= builder.Lte(a => a.Period.EndDate, endDate.Value);
        }

        return await _analytics.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Analytics> AddAsync(Analytics analytics, CancellationToken cancellationToken = default)
    {
        await _analytics.InsertOneAsync(analytics, cancellationToken: cancellationToken);
        return analytics;
    }

    public async Task UpdateAsync(Analytics analytics, CancellationToken cancellationToken = default)
    {
        await _analytics.ReplaceOneAsync(
            a => a.Id == analytics.Id,
            analytics,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _analytics.DeleteOneAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? projectId = null,
        string? source = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Analytics>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(a => a.ProjectId, projectId);
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            filter &= builder.Eq(a => a.Source, source);
        }

        if (startDate.HasValue)
        {
            filter &= builder.Gte(a => a.Period.StartDate, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= builder.Lte(a => a.Period.EndDate, endDate.Value);
        }

        return (int)await _analytics.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}