using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.Posts;

namespace Publishy.Application.Posts.Mappers;

public static class PostMappers
{
    public static PostResponse MapToPostResponse(Post post) =>
        new(
            post.Id,
            post.ProjectId,
            post.Content,
            post.MediaType.ToString(),
            post.ScheduledDate,
            post.Status.ToString(),
            MapToNetworkSpecs(post.NetworkSpecs)
        );

    private static NetworkSpecs MapToNetworkSpecs(NetworkSpecifications specs) =>
        new(
            specs.Twitter != null ? new TwitterSpecs(specs.Twitter.TweetLength) : null,
            specs.LinkedIn != null ? new LinkedInSpecs(specs.LinkedIn.PostType) : null,
            specs.Instagram != null ? new InstagramSpecs(specs.Instagram.ImageDimensions) : null,
            specs.Blog != null ? new BlogSpecs(specs.Blog.Category) : null,
            specs.Newsletter != null ? new NewsletterSpecs(specs.Newsletter.SubjectLine) : null
        );
}