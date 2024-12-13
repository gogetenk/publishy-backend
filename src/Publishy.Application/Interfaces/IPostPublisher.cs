using Ardalis.Result;

namespace Publishy.Application.Interfaces;

public interface IPostPublisher
{
    Task<Result> PublishAsync(string postId, CancellationToken cancellationToken = default);
}