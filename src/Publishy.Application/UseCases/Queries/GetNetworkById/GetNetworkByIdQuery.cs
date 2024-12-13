using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateNetwork;

namespace Publishy.Application.UseCases.Queries.GetNetworkById;

public record GetNetworkByIdQuery(string NetworkId) : Request<Result<NetworkResponse>>;

public class GetNetworkByIdQueryHandler : MediatorRequestHandler<GetNetworkByIdQuery, Result<NetworkResponse>>
{
    private readonly INetworkRepository _networkRepository;

    public GetNetworkByIdQueryHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<NetworkResponse>> Handle(GetNetworkByIdQuery request, CancellationToken cancellationToken)
    {
        var network = await _networkRepository.GetByIdAsync(request.NetworkId, cancellationToken);
        if (network == null)
            return Result.NotFound($"Network with ID {request.NetworkId} was not found");

        return Result.Success((NetworkResponse)network);
    }
}