using Publishy.Application.Domain.AggregateRoots;

namespace Publishy.Application.Interfaces;

public interface ICalendarRepository
{
    Task<Calendar?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Calendar>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        CalendarStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<Calendar> AddAsync(Calendar calendar, CancellationToken cancellationToken = default);
    Task UpdateAsync(Calendar calendar, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(
        string? projectId = null,
        CalendarStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Calendar>> GetSharedWithUserAsync(string userId, CancellationToken cancellationToken = default);
}