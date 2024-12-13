using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;

namespace Publishy.Application.UseCases.Queries.GetMarketingPlanById;

public record GetMarketingPlanByIdQuery(string PlanId) : Request<Result<MarketingPlanResponse>>;

public class GetMarketingPlanByIdQueryHandler : MediatorRequestHandler<GetMarketingPlanByIdQuery, Result<MarketingPlanResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public GetMarketingPlanByIdQueryHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<MarketingPlanResponse>> Handle(GetMarketingPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} was not found");

        return Result.Success((MarketingPlanResponse)plan);
    }
}