using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreatePost;

namespace Publishy.Application.UseCases.Commands.PublishPost;

public record PublishPostCommand(string PostId) : Request<Result<PostResponse>>;

public class PublishPostCommandHandler : MediatorRequestHandler<PublishPostCommand, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;

    public PublishPostCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    protected override async Task<Result<PostResponse>> Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound($"Post with ID {request.PostId} was not found");

        var publishResult = post.Publish();
        if (!publishResult.IsSuccess)
            return Result.Error(publishResult.Errors.ToArray());

        await _postRepository.UpdateAsync(post, cancellationToken);
        return Result.Success((PostResponse)post);
    }
}