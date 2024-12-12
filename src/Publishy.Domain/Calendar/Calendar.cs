using Ardalis.Result;
using Publishy.Domain.Common.Results;
using Publishy.Domain.Common.Services;
using Publishy.Domain.Common.Validation;
using Publishy.Domain.Posts;

namespace Publishy.Domain.Calendar;

public class Calendar
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly List<CalendarEntry> _entries;

    public string Id { get; private set; }
    public string Month { get; private set; }
    public IReadOnlyCollection<CalendarEntry> Entries => _entries.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private Calendar(string month, IDateTimeProvider dateTimeProvider)
    {
        Id = Guid.NewGuid().ToString();
        Month = month;
        _entries = new List<CalendarEntry>();
        _dateTimeProvider = dateTimeProvider;
        CreatedAt = _dateTimeProvider.UtcNow;
        LastUpdatedAt = _dateTimeProvider.UtcNow;
    }

    public static Result<Calendar> Create(string month, IDateTimeProvider dateTimeProvider)
    {
        var dateTimeProviderValidation = DomainValidator.ValidateNotNull(dateTimeProvider, nameof(dateTimeProvider));
        if (!dateTimeProviderValidation.IsSuccess)
            return Result.Error(dateTimeProviderValidation.Errors);

        var monthValidation = DomainValidator.ValidateMonthFormat(month, nameof(month));
        if (!monthValidation.IsSuccess)
            return Result.Error(monthValidation.Errors);

        return Result.Success(new Calendar(month, dateTimeProvider));
    }

    public Result<CalendarEntry> AddEntry(Post post)
    {
        var postValidation = DomainValidator.ValidateNotNull(post, nameof(post));
        if (!postValidation.IsSuccess)
            return Result.Error(postValidation.Errors);

        var monthDate = DateTime.ParseExact(Month, "yyyy-MM", null);
        if (post.ScheduledDate.Year != monthDate.Year || post.ScheduledDate.Month != monthDate.Month)
            return Result.Error("Post scheduled date must be within the calendar month");

        var entryResult = CalendarEntry.Create(post, _dateTimeProvider);
        if (!entryResult.IsSuccess)
            return Result.Error(entryResult.Errors);

        _entries.Add(entryResult.Value);
        LastUpdatedAt = _dateTimeProvider.UtcNow;

        return Result.Success(entryResult.Value);
    }

    public Result<CalendarEntry> UpdateEntry(string entryId, Post updatedPost)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId);
        if (entry == null)
            return Result.NotFound($"Calendar entry with ID {entryId} not found");

        var monthDate = DateTime.ParseExact(Month, "yyyy-MM", null);
        if (updatedPost.ScheduledDate.Year != monthDate.Year || updatedPost.ScheduledDate.Month != monthDate.Month)
            return Result.Error("Updated post scheduled date must be within the calendar month");

        var updateResult = entry.Update(updatedPost);
        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors);

        LastUpdatedAt = _dateTimeProvider.UtcNow;
        return Result.Success(entry);
    }

    public Result RemoveEntry(string entryId)
    {
        var entry = _entries.FirstOrDefault(e => e.Id == entryId);
        if (entry == null)
            return Result.NotFound($"Calendar entry with ID {entryId} not found");

        _entries.Remove(entry);
        LastUpdatedAt = _dateTimeProvider.UtcNow;
        return Result.Success();
    }
}