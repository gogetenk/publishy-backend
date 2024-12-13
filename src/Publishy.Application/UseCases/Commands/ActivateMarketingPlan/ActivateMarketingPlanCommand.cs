using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;

namespace Publishy.Application.UseCases.Commands.ActivateMarketingPlan;

public record ActivateMarketingPlanCommand(string PlanId) : Request<Result<MarketingPlanResponse>>;

public class ActivateMarketingPlanCommandHandler : MediatorRequestHandler<ActivateMarketingPlanCommand, Result<MarketingPlanResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public ActivateMarketingPlanCommandHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<MarketingPlanResponse>> Handle(ActivateMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} was not found");

        var activateResult = plan.Activate();
        if (!activateResult.IsSuccess)
            return Result.Error(activateResult.Errors.ToArray());

        await _marketingPlanRepository.UpdateAsync(plan, cancellationToken);
        return Result.Success((MarketingPlanResponse)plan);
    }
}