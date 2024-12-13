using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;

namespace Publishy.Application.UseCases.Commands.DeactivateMarketingPlan;

public record DeactivateMarketingPlanCommand(string PlanId) : Request<Result<MarketingPlanResponse>>;

public class DeactivateMarketingPlanCommandHandler : MediatorRequestHandler<DeactivateMarketingPlanCommand, Result<MarketingPlanResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public DeactivateMarketingPlanCommandHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<MarketingPlanResponse>> Handle(DeactivateMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} was not found");

        var deactivateResult = plan.Deactivate();
        if (!deactivateResult.IsSuccess)
            return Result.Error(deactivateResult.Errors.ToArray());

        await _marketingPlanRepository.UpdateAsync(plan, cancellationToken);
        return Result.Success((MarketingPlanResponse)plan);
    }
}