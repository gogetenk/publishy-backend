using Ardalis.Result;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Publishy.Api.Modules.Analytics.Queries;
using Publishy.Api.Modules.Analytics.Responses;
using Publishy.Domain.Common.Results;

namespace Publishy.Api.Modules.Analytics;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/analytics")
            .WithTags("Analytics")
            .WithOpenApi();

        // GET /analytics/global
        group.MapGet("/global", async (IMediator mediator) =>
        {
            var query = new GetGlobalPerformanceQuery();
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetGlobalPerformance")
        .WithSummary("Retrieve global statistics")
        .WithDescription("Fetches global statistics of publications.")
        .Produces<Result<GlobalPerformanceResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /analytics/distribution
        group.MapGet("/distribution", async (IMediator mediator) =>
        {
            var query = new GetNetworkDistributionQuery();
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetNetworkDistribution")
        .WithSummary("Retrieve publication distribution by network")
        .WithDescription("Fetches the proportion of publications across social platforms, broken down by media type.")
        .Produces<Result<NetworkDistributionResponse[]>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /analytics/scheduled-posts
        group.MapGet("/scheduled-posts", async (IMediator mediator) =>
        {
            var query = new GetScheduledPostsCountQuery();
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetScheduledPostsCount")
        .WithSummary("Retrieve total scheduled publications")
        .WithDescription("Fetches the total number of scheduled publications.")
        .Produces<Result<ScheduledPostsCountResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}