using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Publishy.Application.UseCases.Commands.ActivateMarketingPlan;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;
using Publishy.Application.UseCases.Commands.DeactivateMarketingPlan;
using Publishy.Application.UseCases.Commands.DeleteMarketingPlan;
using Publishy.Application.UseCases.Commands.UpdateMarketingPlan;
using Publishy.Application.UseCases.Queries.GetMarketingPlanById;
using Publishy.Application.UseCases.Queries.GetMarketingPlans;

namespace Publishy.Api.Endpoints;

public static class MarketingPlanEndpoints
{
    public static IEndpointRouteBuilder MapMarketingPlanEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/marketing-plans")
            .WithTags("Marketing Plans")
            .WithOpenApi();

        // GET /marketing-plans
        group.MapGet("/", async ([FromServices] IMediator mediator, [FromQuery] int? page, [FromQuery] int? pageSize,
            [FromQuery] string? projectId, [FromQuery] string? status,
            [FromQuery] DateTime? startDateAfter, [FromQuery] DateTime? startDateBefore) =>
        {
            var query = new GetMarketingPlansQuery(page ?? 1, pageSize ?? 10, projectId, status, startDateAfter, startDateBefore);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetMarketingPlans")
        .WithSummary("Retrieve the list of marketing plans")
        .WithDescription("Fetches all marketing plans with their details.")
        .Produces<Result<GetMarketingPlansResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /marketing-plans
        group.MapPost("/", async ([FromServices] IMediator mediator, CreateMarketingPlanCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreateMarketingPlan")
        .WithSummary("Create a new marketing plan")
        .WithDescription("Creates a new marketing plan with the provided information.")
        .Produces<Result<MarketingPlanResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /marketing-plans/{planId}
        group.MapGet("/{planId}", async ([FromServices] IMediator mediator, string planId) =>
        {
            var query = new GetMarketingPlanByIdQuery(planId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetMarketingPlanById")
        .WithSummary("Retrieve marketing plan details")
        .WithDescription("Fetches details of a specific marketing plan.")
        .Produces<Result<MarketingPlanResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /marketing-plans/{planId}
        group.MapPut("/{planId}", async ([FromServices] IMediator mediator, string planId, UpdateMarketingPlanCommand command) =>
        {
            command = command with { PlanId = planId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdateMarketingPlan")
        .WithSummary("Update a marketing plan")
        .WithDescription("Updates information of an existing marketing plan.")
        .Produces<Result<MarketingPlanResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /marketing-plans/{planId}
        group.MapDelete("/{planId}", async ([FromServices] IMediator mediator, string planId) =>
        {
            var command = new DeleteMarketingPlanCommand(planId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("DeleteMarketingPlan")
        .WithSummary("Delete a marketing plan")
        .WithDescription("Deletes an existing marketing plan.")
        .Produces<Result>(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /marketing-plans/{planId}/activate
        group.MapPost("/{planId}/activate", async ([FromServices] IMediator mediator, string planId) =>
        {
            var command = new ActivateMarketingPlanCommand(planId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("ActivateMarketingPlan")
        .WithSummary("Activate a marketing plan")
        .WithDescription("Activates an existing marketing plan.")
        .Produces<Result<MarketingPlanResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /marketing-plans/{planId}/deactivate
        group.MapPost("/{planId}/deactivate", async ([FromServices] IMediator mediator, string planId) =>
        {
            var command = new DeactivateMarketingPlanCommand(planId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("DeactivateMarketingPlan")
        .WithSummary("Deactivate a marketing plan")
        .WithDescription("Deactivates an existing marketing plan.")
        .Produces<Result<MarketingPlanResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}