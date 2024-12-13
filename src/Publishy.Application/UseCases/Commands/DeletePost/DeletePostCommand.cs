using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.DeletePost;

public record DeletePostCommand(string PostId) : Request<Result>;

public class DeletePostCommandHandler : MediatorRequestHandler<DeletePostCommand, Result>
{
    private readonly IPostRepository _postRepository;

    public DeletePostCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    protected override async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound($"Post with ID {request.PostId} was not found");

        await _postRepository.DeleteAsync(request.PostId, cancellationToken);
        return Result.Success();
    }
}