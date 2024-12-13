using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.CreateMarketingPlan;

public record CreateMarketingPlanCommand(
    string ProjectId,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    List<MarketingGoalDto> Goals,
    List<ContentStrategyDto> ContentStrategies
) : Request<Result<MarketingPlanResponse>>;

public record MarketingGoalDto(
    string Name,
    string Description,
    string MetricType,
    decimal TargetValue,
    DateTime TargetDate
);

public record ContentStrategyDto(
    string Platform,
    string ContentType,
    string Frequency,
    string TargetAudience,
    string ToneOfVoice,
    List<string> KeyTopics
);

public class CreateMarketingPlanCommandHandler : MediatorRequestHandler<CreateMarketingPlanCommand, Result<MarketingPlanResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateMarketingPlanCommandHandler(IMarketingPlanRepository marketingPlanRepository, IProjectRepository projectRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<MarketingPlanResponse>> Handle(CreateMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        var goals = request.Goals
            .Select(g => new MarketingGoal(g.Name, g.Description, g.MetricType, g.TargetValue, g.TargetDate))
            .ToList();

        var contentStrategies = request.ContentStrategies
            .Select(cs => new ContentStrategy(cs.Platform, cs.ContentType, cs.Frequency, cs.TargetAudience, cs.ToneOfVoice, cs.KeyTopics))
            .ToList();

        var planResult = Domain.AggregateRoots.MarketingPlan.Create(
            request.ProjectId,
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
            goals,
            contentStrategies
        );

        if (!planResult.IsSuccess)
            return Result.Error(planResult.Errors.ToArray());

        var plan = await _marketingPlanRepository.AddAsync(planResult.Value, cancellationToken);
        return Result.Success((MarketingPlanResponse)plan);
    }
}