using Ardalis.Result;
using MassTransit;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.UseCases.Messages;

namespace Publishy.Application.UseCases.Commands.CancelPost;

public record CancelPostCommand(string PostId) : Request<Result<PostResponse>>;

public class CancelPostCommandHandler : MediatorRequestHandler<CancelPostCommand, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CancelPostCommandHandler(
        IPostRepository postRepository,
        IPublishEndpoint publishEndpoint)
    {
        _postRepository = postRepository;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task<Result<PostResponse>> Handle(CancelPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound($"Post with ID {request.PostId} was not found");

        var cancelResult = post.Cancel();
        if (!cancelResult.IsSuccess)
            return Result.Error(cancelResult.Errors.ToArray());

        // Publier un message d'annulation pour supprimer le message planifié
        await _publishEndpoint.Publish(new CancelScheduledPostMessage(post.Id), cancellationToken);

        await _postRepository.UpdateAsync(post, cancellationToken);
        return Result.Success((PostResponse)post);
    }
}