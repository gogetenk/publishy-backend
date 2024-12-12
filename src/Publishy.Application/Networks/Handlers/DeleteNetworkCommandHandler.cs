using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Networks.Commands;
using Publishy.Domain.Networks;

namespace Publishy.Application.Networks.Handlers;

public class DeleteNetworkCommandHandler : MediatorRequestHandler<DeleteNetworkCommand, Result>
{
    private readonly INetworkRepository _networkRepository;

    public DeleteNetworkCommandHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result> Handle(DeleteNetworkCommand request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.NetworkId, cancellationToken);
        if (network == null)
            return Result.NotFound($"Network with ID {request.NetworkId} not found");

        var disconnectResult = network.Disconnect();
        if (!disconnectResult.IsSuccess)
            return Result.Error(disconnectResult.Errors);

        await _networkRepository.DeleteAsync(network.Id, cancellationToken);
        return Result.Success();
    }
}