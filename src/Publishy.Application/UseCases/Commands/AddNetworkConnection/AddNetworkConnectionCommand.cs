using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateNetwork;

namespace Publishy.Application.UseCases.Commands.AddNetworkConnection;

public record AddNetworkConnectionCommand(
    string NetworkId,
    NetworkConnectionDto Connection
) : Request<Result<NetworkResponse>>;

public class AddNetworkConnectionCommandHandler : MediatorRequestHandler<AddNetworkConnectionCommand, Result<NetworkResponse>>
{
    private readonly INetworkRepository _networkRepository;

    public AddNetworkConnectionCommandHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<NetworkResponse>> Handle(AddNetworkConnectionCommand request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.NetworkId, cancellationToken);
        if (network == null)
            return Result.NotFound($"Network with ID {request.NetworkId} was not found");

        var connection = new NetworkConnection(
            request.Connection.SourceId,
            request.Connection.TargetId,
            request.Connection.Type,
            request.Connection.Strength,
            request.Connection.Metadata
        );

        var addResult = network.AddConnection(connection);
        if (!addResult.IsSuccess)
            return Result.Error(addResult.Errors.ToArray());

        await _networkRepository.UpdateAsync(network, cancellationToken);
        return Result.Success((NetworkResponse)network);
    }
}