using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;

namespace Publishy.Application.UseCases.Commands.UpdateMarketingPlan;

public record UpdateMarketingPlanCommand(
    string PlanId,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    List<MarketingGoalDto> Goals,
    List<ContentStrategyDto> ContentStrategies
) : Request<Result<MarketingPlanResponse>>;

public class UpdateMarketingPlanCommandHandler : MediatorRequestHandler<UpdateMarketingPlanCommand, Result<MarketingPlanResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public UpdateMarketingPlanCommandHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<MarketingPlanResponse>> Handle(UpdateMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} was not found");

        var goals = request.Goals
            .Select(g => new MarketingGoal(g.Name, g.Description, g.MetricType, g.TargetValue, g.TargetDate))
            .ToList();

        var contentStrategies = request.ContentStrategies
            .Select(cs => new ContentStrategy(cs.Platform, cs.ContentType, cs.Frequency, cs.TargetAudience, cs.ToneOfVoice, cs.KeyTopics))
            .ToList();

        var updateResult = plan.Update(
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
            goals,
            contentStrategies
        );

        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors.ToArray());

        await _marketingPlanRepository.UpdateAsync(plan, cancellationToken);
        return Result.Success((MarketingPlanResponse)plan);
    }
}