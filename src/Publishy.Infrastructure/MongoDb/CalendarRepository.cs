using MongoDB.Driver;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.MongoDb;

public class CalendarRepository : ICalendarRepository
{
    private readonly IMongoCollection<Calendar> _calendars;

    public CalendarRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("publishy-db");
        _calendars = database.GetCollection<Calendar>("Calendars");
    }

    public async Task<Calendar?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _calendars.Find(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Calendar>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        CalendarStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Calendar>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(c => c.ProjectId, projectId);
        }

        if (status.HasValue)
        {
            filter &= builder.Eq(c => c.Status, status);
        }

        return await _calendars.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Calendar> AddAsync(Calendar calendar, CancellationToken cancellationToken = default)
    {
        await _calendars.InsertOneAsync(calendar, cancellationToken: cancellationToken);
        return calendar;
    }

    public async Task UpdateAsync(Calendar calendar, CancellationToken cancellationToken = default)
    {
        await _calendars.ReplaceOneAsync(
            c => c.Id == calendar.Id,
            calendar,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _calendars.DeleteOneAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? projectId = null,
        CalendarStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Calendar>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(c => c.ProjectId, projectId);
        }

        if (status.HasValue)
        {
            filter &= builder.Eq(c => c.Status, status);
        }

        return (int)await _calendars.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Calendar>> GetSharedWithUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _calendars.Find(c => c.SharedWith.Contains(userId))
            .ToListAsync(cancellationToken);
    }
}