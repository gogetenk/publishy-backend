using Ardalis.Result;
using Publishy.Domain.Common.Results;
using Publishy.Domain.Common.Services;
using Publishy.Domain.Common.Validation;
using Publishy.Domain.Posts;

namespace Publishy.Domain.Calendar;

public class CalendarEntry
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public string Id { get; private set; }
    public string PostId { get; private set; }
    public string ProjectId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public MediaType MediaType { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public PostStatus Status { get; private set; }
    public NetworkSpecifications NetworkSpecs { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private CalendarEntry(Post post, string title, IDateTimeProvider dateTimeProvider)
    {
        Id = Guid.NewGuid().ToString();
        PostId = post.Id;
        ProjectId = post.ProjectId;
        Title = title;
        Content = post.Content;
        MediaType = post.MediaType;
        ScheduledDate = post.ScheduledDate;
        Status = post.Status;
        NetworkSpecs = post.NetworkSpecs;
        _dateTimeProvider = dateTimeProvider;
        CreatedAt = _dateTimeProvider.UtcNow;
        LastUpdatedAt = _dateTimeProvider.UtcNow;
    }

    public static Result<CalendarEntry> Create(Post post, IDateTimeProvider dateTimeProvider)
    {
        var postValidation = DomainValidator.ValidateNotNull(post, nameof(post));
        if (!postValidation.IsSuccess)
            return Result.Error(postValidation.Errors);

        var dateTimeProviderValidation = DomainValidator.ValidateNotNull(dateTimeProvider, nameof(dateTimeProvider));
        if (!dateTimeProviderValidation.IsSuccess)
            return Result.Error(dateTimeProviderValidation.Errors);

        var postIdValidation = DomainValidator.ValidateString(post.Id, nameof(post.Id));
        if (!postIdValidation.IsSuccess)
            return Result.Error(postIdValidation.Errors);

        var title = $"Post for {post.ScheduledDate:MMM dd, yyyy}";
        return Result.Success(new CalendarEntry(post, title, dateTimeProvider));
    }

    public Result Update(Post post)
    {
        var postValidation = DomainValidator.ValidateNotNull(post, nameof(post));
        if (!postValidation.IsSuccess)
            return Result.Error(postValidation.Errors);

        if (Status == PostStatus.Canceled)
            return Result.Error("Cannot update a canceled entry");

        if (Status == PostStatus.Published)
            return Result.Error("Cannot update a published entry");

        Content = post.Content;
        MediaType = post.MediaType;
        ScheduledDate = post.ScheduledDate;
        Status = post.Status;
        NetworkSpecs = post.NetworkSpecs;
        LastUpdatedAt = _dateTimeProvider.UtcNow;

        return Result.Success();
    }

    public Result UpdateTitle(string newTitle)
    {
        var titleValidation = DomainValidator.ValidateString(newTitle, nameof(newTitle));
        if (!titleValidation.IsSuccess)
            return Result.Error(titleValidation.Errors);

        Title = newTitle;
        LastUpdatedAt = _dateTimeProvider.UtcNow;
        return Result.Success();
    }
}