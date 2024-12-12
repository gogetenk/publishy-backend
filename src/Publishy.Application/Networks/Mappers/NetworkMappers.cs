using Publishy.Api.Modules.Networks.Responses;
using Publishy.Domain.Networks;

namespace Publishy.Application.Networks.Mappers;

public static class NetworkMappers
{
    public static NetworkResponse MapToNetworkResponse(Network network) =>
        new(network.Id, network.Platform);
}