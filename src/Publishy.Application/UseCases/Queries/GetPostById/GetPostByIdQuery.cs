using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreatePost;

namespace Publishy.Application.UseCases.Queries.GetPostById;

public record GetPostByIdQuery(string PostId) : Request<Result<PostResponse>>;

public class GetPostByIdQueryHandler : MediatorRequestHandler<GetPostByIdQuery, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;

    public GetPostByIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    protected override async Task<Result<PostResponse>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound($"Post with ID {request.PostId} was not found");

        return Result.Success((PostResponse)post);
    }
}