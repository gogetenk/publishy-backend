using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.MarketingPlans.Commands;
using Publishy.Api.Modules.MarketingPlans.Responses;
using Publishy.Domain.MarketingPlans;
using Publishy.Application.MarketingPlans.Mappers;

namespace Publishy.Application.MarketingPlans.Handlers;

public class FinalizeMarketingPlanCommandHandler : MediatorRequestHandler<FinalizeMarketingPlanCommand, Result<MarketingPlanResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public FinalizeMarketingPlanCommandHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<MarketingPlanResponse>> Handle(FinalizeMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} not found");

        var finalizeResult = plan.Finalize();
        if (!finalizeResult.IsSuccess)
            return Result.Error(finalizeResult.Errors);

        await _marketingPlanRepository.UpdateAsync(plan, cancellationToken);
        return Result.Success(MarketingPlanMappers.MapToMarketingPlanResponse(plan));
    }
}
