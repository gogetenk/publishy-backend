using Ardalis.Result;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.Domain.AggregateRoots;

public class Post
{
    public string Id { get; private set; }
    public string ProjectId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public PostStatus Status { get; private set; }
    public string Platform { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ScheduledFor { get; private set; }
    public List<string> Tags { get; private set; }
    public List<MediaAsset> MediaAssets { get; private set; }

    private Post() { } // For MongoDB

    private Post(
        string projectId,
        string title,
        string content,
        string platform,
        DateTime? scheduledFor,
        List<string> tags,
        List<MediaAsset> mediaAssets)
    {
        Id = Guid.NewGuid().ToString();
        ProjectId = projectId;
        Title = title;
        Content = content;
        Status = PostStatus.Draft;
        Platform = platform;
        CreatedAt = DateTime.UtcNow;
        ScheduledFor = scheduledFor;
        Tags = tags;
        MediaAssets = mediaAssets;
    }

    public static Result<Post> Create(
        string projectId,
        string title,
        string content,
        string platform,
        DateTime? scheduledFor,
        List<string> tags,
        List<MediaAsset> mediaAssets)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return Result.Error("Project ID cannot be empty");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Error("Post title cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Error("Post content cannot be empty");

        if (string.IsNullOrWhiteSpace(platform))
            return Result.Error("Platform cannot be empty");

        if (scheduledFor.HasValue && scheduledFor.Value <= DateTime.UtcNow)
            return Result.Error("Scheduled date must be in the future");

        return Result.Success(new Post(projectId, title, content, platform, scheduledFor, tags ?? new(), mediaAssets ?? new()));
    }

    public Result Update(
        string title,
        string content,
        string platform,
        DateTime? scheduledFor,
        List<string> tags,
        List<MediaAsset> mediaAssets)
    {
        if (Status == PostStatus.Published)
            return Result.Error("Cannot update a published post");

        if (string.IsNullOrWhiteSpace(title))
            return Result.Error("Post title cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Error("Post content cannot be empty");

        if (string.IsNullOrWhiteSpace(platform))
            return Result.Error("Platform cannot be empty");

        if (scheduledFor.HasValue && scheduledFor.Value <= DateTime.UtcNow)
            return Result.Error("Scheduled date must be in the future");

        Title = title;
        Content = content;
        Platform = platform;
        ScheduledFor = scheduledFor;
        Tags = tags ?? new();
        MediaAssets = mediaAssets ?? new();

        return Result.Success();
    }

    public Result Publish()
    {
        if (Status == PostStatus.Published)
            return Result.Error("Post is already published");

        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Schedule(DateTime scheduledFor)
    {
        if (Status == PostStatus.Published)
            return Result.Error("Cannot schedule a published post");

        if (scheduledFor <= DateTime.UtcNow)
            return Result.Error("Scheduled date must be in the future");

        Status = PostStatus.Scheduled;
        ScheduledFor = scheduledFor;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == PostStatus.Published)
            return Result.Error("Cannot cancel a published post");

        Status = PostStatus.Draft;
        ScheduledFor = null;
        return Result.Success();
    }
}

public enum PostStatus
{
    Draft,
    Scheduled,
    Published
}