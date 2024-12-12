using Ardalis.Result;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Publishy.Api.Modules.Posts.Commands;
using Publishy.Api.Modules.Posts.Queries;
using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.Common.Results;

namespace Publishy.Api.Modules.Posts;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId}/posts")
            .WithTags("Posts")
            .WithOpenApi();

        // GET /projects/{projectId}/posts
        group.MapGet("/", async (IMediator mediator, string projectId, string? status, string? mediaType) =>
        {
            var query = new GetProjectPostsQuery(projectId, status, mediaType);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetProjectPosts")
        .WithSummary("Retrieve posts for a project")
        .WithDescription("Fetches all posts (scheduled, published) for a specific project.")
        .Produces<Result<PostResponse[]>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /projects/{projectId}/posts
        group.MapPost("/", async (IMediator mediator, string projectId, CreatePostCommand command) =>
        {
            command = command with { ProjectId = projectId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreateProjectPost")
        .WithSummary("Create a new post for a project")
        .WithDescription("Creates and schedules a new post for a specific project.")
        .Produces<Result<PostResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}