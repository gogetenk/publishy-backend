using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MassTransit;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.UseCases.Commands.AddNetworkConnection;
using Publishy.Application.UseCases.Commands.CreateNetwork;
using Publishy.Application.UseCases.Commands.RemoveNetworkConnection;
using Publishy.Application.UseCases.Commands.UpdateNetworkStatus;
using Publishy.Application.UseCases.Queries.GetNetworkById;
using Publishy.Application.UseCases.Queries.GetNetworks;

namespace Publishy.Api.Endpoints;

public static class NetworkEndpoints
{
    public static IEndpointRouteBuilder MapNetworkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/networks")
            .WithTags("Networks")
            .WithOpenApi();

        // GET /networks
        group.MapGet("/", async ([FromServices] IMediator mediator, [FromQuery] int? page, [FromQuery] int? pageSize,
            [FromQuery] string? projectId, [FromQuery] NetworkStatus? status) =>
        {
            var query = new GetNetworksQuery(page ?? 1, pageSize ?? 10, projectId, status);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetNetworks")
        .WithSummary("Retrieve the list of networks")
        .WithDescription("Fetches all networks with their details.")
        .Produces<Result<GetNetworksResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /networks
        group.MapPost("/", async ([FromServices] IMediator mediator, CreateNetworkCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreateNetwork")
        .WithSummary("Create a new network")
        .WithDescription("Creates a new network with the provided information.")
        .Produces<Result<NetworkResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /networks/{networkId}
        group.MapGet("/{networkId}", async ([FromServices] IMediator mediator, string networkId) =>
        {
            var query = new GetNetworkByIdQuery(networkId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetNetworkById")
        .WithSummary("Retrieve network details")
        .WithDescription("Fetches details of a specific network.")
        .Produces<Result<NetworkResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /networks/{networkId}/connections
        group.MapPost("/{networkId}/connections", async ([FromServices] IMediator mediator, string networkId, NetworkConnectionDto connection) =>
        {
            var command = new AddNetworkConnectionCommand(networkId, connection);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("AddNetworkConnection")
        .WithSummary("Add a connection to the network")
        .WithDescription("Adds a new connection between nodes in the network.")
        .Produces<Result<NetworkResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /networks/{networkId}/connections
        group.MapDelete("/{networkId}/connections", async ([FromServices] IMediator mediator, string networkId,
            [FromQuery] string sourceId, [FromQuery] string targetId, [FromQuery] string type) =>
        {
            var command = new RemoveNetworkConnectionCommand(networkId, sourceId, targetId, type);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("RemoveNetworkConnection")
        .WithSummary("Remove a connection from the network")
        .WithDescription("Removes an existing connection between nodes in the network.")
        .Produces<Result<NetworkResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /networks/{networkId}/status
        group.MapPut("/{networkId}/status", async ([FromServices] IMediator mediator, string networkId, UpdateNetworkStatusCommand command) =>
        {
            command = command with { NetworkId = networkId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdateNetworkStatus")
        .WithSummary("Update network status")
        .WithDescription("Activates or deactivates a network.")
        .Produces<Result<NetworkResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}