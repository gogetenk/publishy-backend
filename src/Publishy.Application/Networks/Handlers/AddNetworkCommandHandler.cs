using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Networks.Commands;
using Publishy.Api.Modules.Networks.Responses;
using Publishy.Domain.Networks;
using Publishy.Application.Networks.Mappers;

namespace Publishy.Application.Networks.Handlers;

public class AddNetworkCommandHandler : MediatorRequestHandler<AddNetworkCommand, Result<NetworkResponse>>
{
    private readonly INetworkRepository _networkRepository;

    public AddNetworkCommandHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<NetworkResponse>> Handle(AddNetworkCommand request, CancellationToken cancellationToken)
    {
        var credentials = NetworkCredentials.Create(
            request.Credentials.ClientId,
            request.Credentials.ClientSecret,
            request.Credentials.AccessToken,
            request.Credentials.RefreshToken
        );

        if (!credentials.IsSuccess)
            return Result.Error(credentials.Errors);

        var networkResult = Network.Create(request.Platform, credentials.Value);
        if (!networkResult.IsSuccess)
            return Result.Error(networkResult.Errors);

        var network = await _networkRepository.AddAsync(networkResult.Value, cancellationToken);
        return Result.Success(NetworkMappers.MapToNetworkResponse(network));
    }
}