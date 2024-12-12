using MassTransit;
using Publishy.Api.Modules.Posts.Responses;

namespace Publishy.Api.Modules.Posts.Commands;

public record CreatePostCommand(
    string ProjectId,
    string Content,
    string MediaType,
    DateTime ScheduledDate,
    NetworkSpecs NetworkSpecs
) : Request<PostResponse>;

public record NetworkSpecs(
    TwitterSpecs? Twitter,
    LinkedInSpecs? LinkedIn,
    InstagramSpecs? Instagram,
    BlogSpecs? Blog,
    NewsletterSpecs? Newsletter
);

public record TwitterSpecs(int TweetLength);
public record LinkedInSpecs(string PostType);
public record InstagramSpecs(string ImageDimensions);
public record BlogSpecs(string Category);
public record NewsletterSpecs(string SubjectLine);