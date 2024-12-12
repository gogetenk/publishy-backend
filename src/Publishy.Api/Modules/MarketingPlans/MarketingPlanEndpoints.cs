using Ardalis.Result;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Publishy.Api.Modules.MarketingPlans.Commands;
using Publishy.Api.Modules.MarketingPlans.Queries;
using Publishy.Api.Modules.MarketingPlans.Responses;
using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.Common.Results;

namespace Publishy.Api.Modules.MarketingPlans;

public static class MarketingPlanEndpoints
{
    public static IEndpointRouteBuilder MapMarketingPlanEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/marketing-plans")
            .WithTags("MarketingPlans")
            .WithOpenApi();

        // GET /marketing-plans/{planId}/posts
        group.MapGet("/{planId}/posts", async (IMediator mediator, string planId) =>
        {
            var query = new GetMarketingPlanPostsQuery(planId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetMarketingPlanPosts")
        .WithSummary("Retrieve proposed posts for a marketing plan")
        .WithDescription("Fetches automatically generated posts for a specific marketing plan.")
        .Produces<Result<PostResponse[]>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /marketing-plans/{planId}/posts
        group.MapPost("/{planId}/posts", async (IMediator mediator, string planId, AddPostToMarketingPlanCommand command) =>
        {
            command = command with { PlanId = planId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("AddPostToMarketingPlan")
        .WithSummary("Add a post to a marketing plan")
        .WithDescription("Adds a new post to the marketing plan.")
        .Produces<Result<PostResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /marketing-plans/{planId}/timeline
        group.MapGet("/{planId}/timeline", async (IMediator mediator, string planId) =>
        {
            var query = new GetMarketingPlanTimelineQuery(planId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetMarketingPlanTimeline")
        .WithSummary("Retrieve the monthly timeline of a marketing plan")
        .WithDescription("Fetches the scheduled publications calendar for a specific marketing plan.")
        .Produces<Result<TimelineResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /marketing-plans/{planId}/timeline
        group.MapPost("/{planId}/timeline", async (IMediator mediator, string planId, AddPostToTimelineCommand command) =>
        {
            command = command with { PlanId = planId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("AddPostToTimeline")
        .WithSummary("Add a publication to the timeline")
        .WithDescription("Adds a publication to the marketing plan's timeline.")
        .Produces<Result<TimelineEntryResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /marketing-plans/{planId}/finalize
        group.MapPost("/{planId}/finalize", async (IMediator mediator, string planId) =>
        {
            var command = new FinalizeMarketingPlanCommand(planId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("FinalizeMarketingPlan")
        .WithSummary("Finalize and publish a marketing plan")
        .WithDescription("Finalizes the marketing plan and publishes the scheduled publications.")
        .Produces<Result<MarketingPlanResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}