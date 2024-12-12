using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Analytics.Queries;
using Publishy.Api.Modules.Analytics.Responses;
using Publishy.Domain.Analytics;
using Publishy.Application.Analytics.Mappers;

namespace Publishy.Application.Analytics.Handlers;

public class GetGlobalPerformanceQueryHandler : MediatorRequestHandler<GetGlobalPerformanceQuery, Result<GlobalPerformanceResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetGlobalPerformanceQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<GlobalPerformanceResponse>> Handle(GetGlobalPerformanceQuery request, CancellationToken cancellationToken)
    {
        var performance = await _analyticsRepository.GetGlobalPerformanceAsync(cancellationToken);
        if (performance == null)
            return Result.NotFound("Global performance metrics not found");

        return Result.Success(AnalyticsMappers.MapToGlobalPerformanceResponse(performance));
    }
}