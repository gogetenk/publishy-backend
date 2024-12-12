using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Analytics.Queries;
using Publishy.Api.Modules.Analytics.Responses;
using Publishy.Domain.Analytics;
using Publishy.Application.Analytics.Mappers;

namespace Publishy.Application.Analytics.Handlers;

public class GetNetworkDistributionQueryHandler : MediatorRequestHandler<GetNetworkDistributionQuery, Result<NetworkDistributionResponse[]>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetNetworkDistributionQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<NetworkDistributionResponse[]>> Handle(GetNetworkDistributionQuery request, CancellationToken cancellationToken)
    {
        var distributions = await _analyticsRepository.GetNetworkDistributionAsync(cancellationToken);
        if (!distributions.Any())
            return Result.NotFound("Network distribution metrics not found");

        return Result.Success(distributions.Select(AnalyticsMappers.MapToNetworkDistributionResponse).ToArray());
    }
}