using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Common.Responses;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateNetwork;

namespace Publishy.Application.UseCases.Queries.GetNetworks;

public record GetNetworksQuery(
    int Page,
    int PageSize,
    string? ProjectId,
    string? Status
) : Request<Result<GetNetworksResponse>>;

public class GetNetworksQueryHandler : MediatorRequestHandler<GetNetworksQuery, Result<GetNetworksResponse>>
{
    private readonly INetworkRepository _networkRepository;

    public GetNetworksQueryHandler(INetworkRepository networkRepository)
    {
        _networkRepository = networkRepository;
    }

    protected override async Task<Result<GetNetworksResponse>> Handle(GetNetworksQuery request, CancellationToken cancellationToken)
    {
        var networks = await _networkRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.ProjectId,
            request.Status,
            cancellationToken
        );

        var totalItems = await _networkRepository.GetTotalCountAsync(
            request.ProjectId,
            request.Status,
            cancellationToken
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var networkResponses = networks.Select(n => (NetworkResponse)n).ToArray();

        var response = new GetNetworksResponse(
            Data: networkResponses,
            Pagination: new PaginationResponse(
                CurrentPage: request.Page,
                PageSize: request.PageSize,
                TotalPages: totalPages,
                TotalItems: totalItems
            )
        );

        return Result.Success(response);
    }
}