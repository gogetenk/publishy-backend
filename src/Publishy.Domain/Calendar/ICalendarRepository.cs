namespace Publishy.Domain.Calendar;

public interface ICalendarRepository
{
    Task<Calendar?> GetByMonthAsync(string month, CancellationToken cancellationToken = default);
    Task<Calendar> AddAsync(Calendar calendar, CancellationToken cancellationToken = default);
    Task UpdateAsync(Calendar calendar, CancellationToken cancellationToken = default);
    Task<CalendarEntry?> GetEntryByPostIdAsync(string postId, CancellationToken cancellationToken = default);
    Task DeleteEntryAsync(string entryId, CancellationToken cancellationToken = default);
}