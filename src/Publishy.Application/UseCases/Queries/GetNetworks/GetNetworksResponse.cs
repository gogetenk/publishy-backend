using Publishy.Application.Common.Responses;
using Publishy.Application.UseCases.Commands.CreateNetwork;

namespace Publishy.Application.UseCases.Queries.GetNetworks;

public record GetNetworksResponse(
    NetworkResponse[] Data,
    PaginationResponse Pagination
);