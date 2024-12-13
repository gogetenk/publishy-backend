using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.Common.Responses;

namespace Publishy.Application.UseCases.Queries.GetPosts;

public record GetPostsResponse(
    PostResponse[] Data,
    PaginationResponse Pagination
);