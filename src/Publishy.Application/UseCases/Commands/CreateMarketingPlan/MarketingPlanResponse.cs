using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Commands.CreateMarketingPlan;

public record MarketingPlanResponse(
    string Id,
    string ProjectId,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    List<MarketingGoal> Goals,
    List<ContentStrategy> ContentStrategies,
    DateTime CreatedAt,
    DateTime LastModifiedAt
)
{
    public static explicit operator MarketingPlanResponse(Domain.AggregateRoots.MarketingPlan plan)
    {
        return new MarketingPlanResponse(
            plan.Id,
            plan.ProjectId,
            plan.Name,
            plan.Description,
            plan.StartDate,
            plan.EndDate,
            plan.Status.ToString(),
            plan.Goals,
            plan.ContentStrategies,
            plan.CreatedAt,
            plan.LastModifiedAt
        );
    }
}