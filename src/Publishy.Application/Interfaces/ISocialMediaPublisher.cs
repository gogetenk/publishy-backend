using Ardalis.Result;

namespace Publishy.Application.Interfaces;

public interface ISocialMediaPublisher
{
    Task<Result> PublishAsync(SocialMediaPost post, CancellationToken cancellationToken = default);
}

public record SocialMediaPost(
    string Content,
    List<string> MediaUrls,
    Dictionary<string, string> Metadata
);