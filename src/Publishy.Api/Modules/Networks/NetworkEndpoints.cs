using Ardalis.Result;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Publishy.Api.Modules.Networks.Commands;
using Publishy.Api.Modules.Networks.Queries;
using Publishy.Api.Modules.Networks.Responses;
using Publishy.Domain.Common.Results;

namespace Publishy.Api.Modules.Networks;

public static class NetworkEndpoints
{
    public static IEndpointRouteBuilder MapNetworkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/networks")
            .WithTags("Networks")
            .WithOpenApi();

        // GET /networks
        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetNetworksQuery();
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetNetworks")
        .WithSummary("Retrieve connected social networks")
        .WithDescription("Fetches the list of currently connected social platforms.")
        .Produces<Result<NetworkResponse[]>>()
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /networks
        group.MapPost("/", async (IMediator mediator, AddNetworkCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("AddNetwork")
        .WithSummary("Add a social platform")
        .WithDescription("Connects a new social platform to the application.")
        .Produces<Result<NetworkResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /networks/{networkId}
        group.MapDelete("/{networkId}", async (IMediator mediator, string networkId) =>
        {
            var command = new DeleteNetworkCommand(networkId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("DeleteNetwork")
        .WithSummary("Disconnect a social platform")
        .WithDescription("Disconnects an existing social platform.")
        .Produces<Result>(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}