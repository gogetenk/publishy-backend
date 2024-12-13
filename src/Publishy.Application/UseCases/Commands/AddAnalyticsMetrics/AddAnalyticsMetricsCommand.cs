using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateAnalytics;

namespace Publishy.Application.UseCases.Commands.AddAnalyticsMetrics;

public record AddAnalyticsMetricsCommand(
    string AnalyticsId,
    List<AnalyticsMetricDto> Metrics
) : Request<Result<AnalyticsResponse>>;

public class AddAnalyticsMetricsCommandHandler : MediatorRequestHandler<AddAnalyticsMetricsCommand, Result<AnalyticsResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public AddAnalyticsMetricsCommandHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<AnalyticsResponse>> Handle(AddAnalyticsMetricsCommand request, CancellationToken cancellationToken)
    {
        var analytics = await _analyticsRepository.GetByIdAsync(request.AnalyticsId, cancellationToken);
        if (analytics == null)
            return Result.NotFound($"Analytics with ID {request.AnalyticsId} was not found");

        var metrics = request.Metrics
            .Select(m => new AnalyticsMetric(
                m.Name,
                m.Category,
                m.Value,
                m.Unit,
                m.Timestamp,
                m.Dimensions
            ))
            .ToList();

        var addResult = analytics.AddMetrics(metrics);
        if (!addResult.IsSuccess)
            return Result.Error(addResult.Errors.ToArray());

        await _analyticsRepository.UpdateAsync(analytics, cancellationToken);
        return Result.Success((AnalyticsResponse)analytics);
    }
}