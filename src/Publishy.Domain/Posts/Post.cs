using Ardalis.Result;
using Publishy.Domain.Projects;

namespace Publishy.Domain.Posts;

public class Post
{
    public string Id { get; private set; }
    public string ProjectId { get; private set; }
    public string Content { get; private set; }
    public MediaType MediaType { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public PostStatus Status { get; private set; }
    public NetworkSpecifications NetworkSpecs { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Post() { } // For EF Core

    private Post(
        string projectId,
        string content,
        MediaType mediaType,
        DateTime scheduledDate,
        NetworkSpecifications networkSpecs)
    {
        Id = Guid.NewGuid().ToString();
        ProjectId = projectId;
        Content = content;
        MediaType = mediaType;
        ScheduledDate = scheduledDate;
        Status = PostStatus.Scheduled;
        NetworkSpecs = networkSpecs;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Post> Create(
        string projectId,
        string content,
        MediaType mediaType,
        DateTime scheduledDate,
        NetworkSpecifications networkSpecs)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return Result.Error("Project ID cannot be empty");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Error("Content cannot be empty");

        if (scheduledDate <= DateTime.UtcNow)
            return Result.Error("Scheduled date must be in the future");

        if (networkSpecs == null)
            return Result.Error("Network specifications are required");

        return Result.Success(new Post(projectId, content, mediaType, scheduledDate, networkSpecs));
    }

    public Result Update(
        string content,
        MediaType mediaType,
        DateTime scheduledDate,
        PostStatus status)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Error("Content cannot be empty");

        if (scheduledDate <= DateTime.UtcNow)
            return Result.Error("Scheduled date must be in the future");

        if (Status == PostStatus.Canceled)
            return Result.Error("Cannot update a canceled post");

        if (Status == PostStatus.Published)
            return Result.Error("Cannot update a published post");

        Content = content;
        MediaType = mediaType;
        ScheduledDate = scheduledDate;
        Status = status;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == PostStatus.Published)
            return Result.Error("Cannot cancel a published post");

        if (Status == PostStatus.Canceled)
            return Result.Error("Post is already canceled");

        Status = PostStatus.Canceled;
        return Result.Success();
    }

    public Result Publish()
    {
        if (Status == PostStatus.Published)
            return Result.Error("Post is already published");

        if (Status == PostStatus.Canceled)
            return Result.Error("Cannot publish a canceled post");

        if (ScheduledDate > DateTime.UtcNow)
            return Result.Error("Cannot publish a post before its scheduled date");

        Status = PostStatus.Published;
        return Result.Success();
    }
}