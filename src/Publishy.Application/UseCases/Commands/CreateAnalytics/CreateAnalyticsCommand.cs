using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.CreateAnalytics;

public record CreateAnalyticsCommand(
    string ProjectId,
    string Source,
    DateTime StartDate,
    DateTime EndDate,
    List<AnalyticsMetricDto> Metrics
) : Request<Result<AnalyticsResponse>>;

public record AnalyticsMetricDto(
    string Name,
    string Category,
    decimal Value,
    string Unit,
    DateTime Timestamp,
    Dictionary<string, string> Dimensions
);

public class CreateAnalyticsCommandHandler : MediatorRequestHandler<CreateAnalyticsCommand, Result<AnalyticsResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateAnalyticsCommandHandler(IAnalyticsRepository analyticsRepository, IProjectRepository projectRepository)
    {
        _analyticsRepository = analyticsRepository;
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<AnalyticsResponse>> Handle(CreateAnalyticsCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        var period = new AnalyticsPeriod(request.StartDate, request.EndDate);
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

        var analyticsResult = Domain.AggregateRoots.Analytics.Create(
            request.ProjectId,
            request.Source,
            period,
            metrics
        );

        if (!analyticsResult.IsSuccess)
            return Result.Error(analyticsResult.Errors.ToArray());

        var analytics = await _analyticsRepository.AddAsync(analyticsResult.Value, cancellationToken);
        return Result.Success((AnalyticsResponse)analytics);
    }
}