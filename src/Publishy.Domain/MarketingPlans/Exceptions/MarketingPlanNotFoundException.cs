using Publishy.Domain.Common.Exceptions;

namespace Publishy.Domain.MarketingPlans.Exceptions;

public class MarketingPlanNotFoundException : DomainException
{
    public MarketingPlanNotFoundException(string planId) 
        : base($"Marketing plan with ID {planId} not found.")
    {
        PlanId = planId;
    }

    public string PlanId { get; }
}