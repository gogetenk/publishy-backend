using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Networks.Queries;
using Publishy.Api.Modules.Networks.Responses;
using Publishy.Domain.Networks;
using Publishy.Application.Networks.Mappers;

namespace Publishy.Application.Networks.Handlers;

public class GetNetworksQueryHandler : MediatorRequestHandler<GetNetworksQuery, Result<NetworkResponse[]>>
{
    private readonly INetworkRepository _networkRepository;

    public GetNetworksQueryHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<NetworkResponse[]>> Handle(GetNetworksQuery request, CancellationToken cancellationToken)
    {
        var networks = await _networkRepository.GetAllAsync(cancellationToken);
        return Result.Success(networks.Select(NetworkMappers.MapToNetworkResponse).ToArray());
    }
}