using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Publishy.Application.UseCases.Commands.AddAnalyticsMetrics;
using Publishy.Application.UseCases.Commands.CreateAnalytics;
using Publishy.Application.UseCases.Queries.GetAnalytics;
using Publishy.Application.UseCases.Queries.GetAnalyticsById;

namespace Publishy.Api.Endpoints;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/analytics")
            .WithTags("Analytics")
            .WithOpenApi();

        // GET /analytics
        group.MapGet("/", async ([FromServices] IMediator mediator, [FromQuery] int? page, [FromQuery] int? pageSize,
            [FromQuery] string? projectId, [FromQuery] string? source, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) =>
        {
            var query = new GetAnalyticsQuery(page ?? 1, pageSize ?? 10, projectId, source, startDate, endDate);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetAnalytics")
        .WithSummary("Retrieve analytics data")
        .WithDescription("Fetches analytics data with optional filtering.")
        .Produces<Result<GetAnalyticsResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /analytics
        group.MapPost("/", async ([FromServices] IMediator mediator, CreateAnalyticsCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreateAnalytics")
        .WithSummary("Create new analytics")
        .WithDescription("Creates a new analytics record with metrics.")
        .Produces<Result<AnalyticsResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /analytics/{analyticsId}
        group.MapGet("/{analyticsId}", async ([FromServices] IMediator mediator, string analyticsId) =>
        {
            var query = new GetAnalyticsByIdQuery(analyticsId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetAnalyticsById")
        .WithSummary("Retrieve analytics details")
        .WithDescription("Fetches details of specific analytics record.")
        .Produces<Result<AnalyticsResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /analytics/{analyticsId}/metrics
        group.MapPost("/{analyticsId}/metrics", async ([FromServices] IMediator mediator, string analyticsId, List<AnalyticsMetricDto> metrics) =>
        {
            var command = new AddAnalyticsMetricsCommand(analyticsId, metrics);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("AddAnalyticsMetrics")
        .WithSummary("Add metrics to analytics")
        .WithDescription("Adds new metrics to an existing analytics record.")
        .Produces<Result<AnalyticsResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}