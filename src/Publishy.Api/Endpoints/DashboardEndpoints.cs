using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Publishy.Application.UseCases.Queries.GetDashboard;

namespace Publishy.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/dashboards")
            .WithTags("Dashboard")
            .WithOpenApi();

        // GET /dashboards
        group.MapGet("/", async ([FromServices] IMediator mediator) =>
        {
            var query = new GetDashboardQuery();
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetDashboard")
        .WithSummary("Retrieve dashboard data")
        .WithDescription("Fetches overview statistics and active projects data for the dashboard.")
        .Produces<Result<DashboardResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
        .CacheOutput("Dashboard");

        return app;
    }
}