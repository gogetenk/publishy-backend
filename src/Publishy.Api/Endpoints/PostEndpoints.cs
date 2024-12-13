using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Publishy.Application.UseCases.Commands.CancelPost;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.UseCases.Commands.DeletePost;
using Publishy.Application.UseCases.Commands.PublishPost;
using Publishy.Application.UseCases.Commands.SchedulePost;
using Publishy.Application.UseCases.Commands.UpdatePost;
using Publishy.Application.UseCases.Queries.GetPostById;
using Publishy.Application.UseCases.Queries.GetPosts;

namespace Publishy.Api.Endpoints;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/posts")
            .WithTags("Posts")
            .WithOpenApi();

        // GET /posts
        group.MapGet("/", async ([FromServices] IMediator mediator, [FromQuery] int? page, [FromQuery] int? pageSize, 
            [FromQuery] string? projectId, [FromQuery] string? status, [FromQuery] string? platform, 
            [FromQuery] DateTime? createdAfter, [FromQuery] DateTime? createdBefore) =>
        {
            var query = new GetPostsQuery(page ?? 1, pageSize ?? 10, projectId, status, platform, createdAfter, createdBefore);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetPosts")
        .WithSummary("Retrieve the list of posts")
        .WithDescription("Fetches all posts with their details.")
        .Produces<Result<GetPostsResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /posts
        group.MapPost("/", async ([FromServices] IMediator mediator, CreatePostCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreatePost")
        .WithSummary("Create a new post")
        .WithDescription("Creates a new post with the provided information.")
        .Produces<Result<PostResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /posts/{postId}
        group.MapGet("/{postId}", async ([FromServices] IMediator mediator, string postId) =>
        {
            var query = new GetPostByIdQuery(postId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetPostById")
        .WithSummary("Retrieve post details")
        .WithDescription("Fetches details of a specific post.")
        .Produces<Result<PostResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /posts/{postId}
        group.MapPut("/{postId}", async ([FromServices] IMediator mediator, string postId, UpdatePostCommand command) =>
        {
            command = command with { PostId = postId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdatePost")
        .WithSummary("Update a post")
        .WithDescription("Updates information of an existing post.")
        .Produces<Result<PostResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /posts/{postId}
        group.MapDelete("/{postId}", async ([FromServices] IMediator mediator, string postId) =>
        {
            var command = new DeletePostCommand(postId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("DeletePost")
        .WithSummary("Delete a post")
        .WithDescription("Deletes an existing post.")
        .Produces<Result>(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /posts/{postId}/publish
        group.MapPost("/{postId}/publish", async ([FromServices] IMediator mediator, string postId) =>
        {
            var command = new PublishPostCommand(postId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("PublishPost")
        .WithSummary("Publish a post")
        .WithDescription("Publishes an existing post.")
        .Produces<Result<PostResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /posts/{postId}/schedule
        group.MapPost("/{postId}/schedule", async ([FromServices] IMediator mediator, string postId, [FromBody] DateTime scheduledFor) =>
        {
            var command = new SchedulePostCommand(postId, scheduledFor);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("SchedulePost")
        .WithSummary("Schedule a post")
        .WithDescription("Schedules an existing post for future publication.")
        .Produces<Result<PostResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /posts/{postId}/cancel
        group.MapPost("/{postId}/cancel", async ([FromServices] IMediator mediator, string postId) =>
        {
            var command = new CancelPostCommand(postId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CancelPost")
        .WithSummary("Cancel a scheduled post")
        .WithDescription("Cancels a scheduled post and returns it to draft status.")
        .Produces<Result<PostResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}