using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateNetwork;

namespace Publishy.Application.UseCases.Commands.UpdateNetworkStatus;

public record UpdateNetworkStatusCommand(
    string NetworkId,
    bool IsActive
) : Request<Result<NetworkResponse>>;

public class UpdateNetworkStatusCommandHandler : MediatorRequestHandler<UpdateNetworkStatusCommand, Result<NetworkResponse>>
{
    private readonly INetworkRepository _networkRepository;

    public UpdateNetworkStatusCommandHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<NetworkResponse>> Handle(UpdateNetworkStatusCommand request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.NetworkId, cancellationToken);
        if (network == null)
            return Result.NotFound($"Network with ID {request.NetworkId} was not found");

        var updateResult = network.UpdateStatus(request.IsActive);
        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors.ToArray());

        await _networkRepository.UpdateAsync(network, cancellationToken);
        return Result.Success((NetworkResponse)network);
    }
}