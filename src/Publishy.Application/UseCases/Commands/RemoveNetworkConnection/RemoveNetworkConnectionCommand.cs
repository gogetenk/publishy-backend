using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateNetwork;

namespace Publishy.Application.UseCases.Commands.RemoveNetworkConnection;

public record RemoveNetworkConnectionCommand(
    string NetworkId,
    string SourceId,
    string TargetId,
    string Type
) : Request<Result<NetworkResponse>>;

public class RemoveNetworkConnectionCommandHandler : MediatorRequestHandler<RemoveNetworkConnectionCommand, Result<NetworkResponse>>
{
    private readonly INetworkRepository _networkRepository;

    public RemoveNetworkConnectionCommandHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<NetworkResponse>> Handle(RemoveNetworkConnectionCommand request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.NetworkId, cancellationToken);
        if (network == null)
            return Result.NotFound($"Network with ID {request.NetworkId} was not found");

        var removeResult = network.RemoveConnection(request.SourceId, request.TargetId, request.Type);
        if (!removeResult.IsSuccess)
            return Result.Error(removeResult.Errors.ToArray());

        await _networkRepository.UpdateAsync(network, cancellationToken);
        return Result.Success((NetworkResponse)network);
    }
}