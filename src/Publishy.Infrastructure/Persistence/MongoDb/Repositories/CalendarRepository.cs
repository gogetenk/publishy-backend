using MongoDB.Driver;
using Publishy.Domain.Calendar;

namespace Publishy.Infrastructure.Persistence.MongoDb.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly IMongoCollection<Calendar> _calendars;

    public CalendarRepository(MongoDbContext context)
    {
        _calendars = context.GetCollection<Calendar>("Calendars");
    }

    public async Task<Calendar?> GetByMonthAsync(string month, CancellationToken cancellationToken = default)
    {
        return await _calendars.Find(c => c.Month == month)
            .FirstOrDefaultAsync(cancellationToken);
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

    public async Task<CalendarEntry?> GetEntryByPostIdAsync(string postId, CancellationToken cancellationToken = default)
    {
        var calendar = await _calendars
            .Find(c => c.Entries.Any(e => e.PostId == postId))
            .FirstOrDefaultAsync(cancellationToken);

        return calendar?.Entries.FirstOrDefault(e => e.PostId == postId);
    }

    public async Task DeleteEntryAsync(string entryId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Calendar>.Update.PullFilter(
            c => c.Entries,
            e => e.Id == entryId);

        await _calendars.UpdateManyAsync(
            c => c.Entries.Any(e => e.Id == entryId),
            update,
            cancellationToken: cancellationToken);
    }
}