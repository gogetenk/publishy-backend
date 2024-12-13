using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Commands.CreatePost;

public record PostResponse(
    string Id,
    string ProjectId,
    string Title,
    string Content,
    string Status,
    string Platform,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    DateTime? ScheduledFor,
    List<string> Tags,
    List<MediaAsset> MediaAssets
)
{
    public static explicit operator PostResponse(Publishy.Application.Domain.AggregateRoots.Post post)
    {
        return new PostResponse(
            post.Id,
            post.ProjectId,
            post.Title,
            post.Content,
            post.Status.ToString(),
            post.Platform,
            post.CreatedAt,
            post.PublishedAt,
            post.ScheduledFor,
            post.Tags,
            post.MediaAssets
        );
    }
}