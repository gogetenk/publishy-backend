using Ardalis.Result;
using Publishy.Domain.Posts;

namespace Publishy.Domain.MarketingPlans.Timeline;

public class TimelineEntry
{
    public string Id { get; private set; }
    public string PostId { get; private set; }
    public string Title { get; private set; }
    public MediaType MediaType { get; private set; }
    public DateTime ScheduledTime { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TimelineEntry() { } // For EF Core

    private TimelineEntry(string postId, string title, MediaType mediaType, DateTime scheduledTime)
    {
        Id = Guid.NewGuid().ToString();
        PostId = postId;
        Title = title;
        MediaType = mediaType;
        ScheduledTime = scheduledTime;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<TimelineEntry> Create(string postId, string title, MediaType mediaType, DateTime scheduledTime)
    {
        if (string.IsNullOrWhiteSpace(postId))
            return Result.Error("Post ID cannot be empty");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Error("Title cannot be empty");

        if (scheduledTime <= DateTime.UtcNow)
            return Result.Error("Scheduled time must be in the future");

        return Result.Success(new TimelineEntry(postId, title, mediaType, scheduledTime));
    }

    public Result Update(string title, DateTime scheduledTime)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Error("Title cannot be empty");

        if (scheduledTime <= DateTime.UtcNow)
            return Result.Error("Scheduled time must be in the future");

        Title = title;
        ScheduledTime = scheduledTime;

        return Result.Success();
    }
}