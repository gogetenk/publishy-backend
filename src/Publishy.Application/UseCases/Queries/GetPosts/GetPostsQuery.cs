using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.Common.Responses;

namespace Publishy.Application.UseCases.Queries.GetPosts;

public record GetPostsQuery(
    int Page,
    int PageSize,
    string? ProjectId,
    string? Status,
    string? Platform,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore
) : Request<Result<GetPostsResponse>>;

public class GetPostsQueryHandler : MediatorRequestHandler<GetPostsQuery, Result<GetPostsResponse>>
{
    private readonly IPostRepository _postRepository;

    public GetPostsQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    protected override async Task<Result<GetPostsResponse>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.ProjectId,
            request.Status,
            request.Platform,
            request.CreatedAfter,
            request.CreatedBefore,
            cancellationToken
        );

        var totalItems = await _postRepository.GetTotalCountAsync(
            request.ProjectId,
            request.Status,
            request.Platform,
            request.CreatedAfter,
            request.CreatedBefore,
            cancellationToken
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var postResponses = posts.Select(post => (PostResponse)post).ToArray();

        var response = new GetPostsResponse(
            Data: postResponses,
            Pagination: new PaginationResponse(
                CurrentPage: request.Page,
                PageSize: request.PageSize,
                TotalPages: totalPages,
                TotalItems: totalItems
            )
        );

        return Result.Success(response);
    }
}