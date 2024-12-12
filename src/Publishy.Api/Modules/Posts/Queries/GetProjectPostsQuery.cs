using MassTransit;
using Publishy.Api.Modules.Posts.Responses;

namespace Publishy.Api.Modules.Posts.Queries;

public record GetProjectPostsQuery(
    string ProjectId,
    string? Status,
    string? MediaType
) : Request<PostResponse[]>;