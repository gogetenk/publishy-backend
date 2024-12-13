using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateAnalytics;

namespace Publishy.Application.UseCases.Queries.GetAnalyticsById;

public record GetAnalyticsByIdQuery(string AnalyticsId) : Request<Result<AnalyticsResponse>>;

public class GetAnalyticsByIdQueryHandler : MediatorRequestHandler<GetAnalyticsByIdQuery, Result<AnalyticsResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetAnalyticsByIdQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<AnalyticsResponse>> Handle(GetAnalyticsByIdQuery request, CancellationToken cancellationToken)
    {
        var analytics = await _analyticsRepository.GetByIdAsync(request.AnalyticsId, cancellationToken);
        if (analytics == null)
            return Result.NotFound($"Analytics with ID {request.AnalyticsId} was not found");

        return Result.Success((AnalyticsResponse)analytics);
    }
}