```csharp
using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.MarketingPlans.Queries;
using Publishy.Api.Modules.MarketingPlans.Responses;
using Publishy.Domain.MarketingPlans;
using Publishy.Application.MarketingPlans.Mappers;

namespace Publishy.Application.MarketingPlans.Handlers;

public class GetMarketingPlanTimelineQueryHandler : MediatorRequestHandler<GetMarketingPlanTimelineQuery, Result<TimelineResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public GetMarketingPlanTimelineQueryHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<TimelineResponse>> Handle(GetMarketingPlanTimelineQuery request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} not found");

        return Result.Success(MarketingPlanMappers.MapToTimelineResponse(plan));
    }
}
```