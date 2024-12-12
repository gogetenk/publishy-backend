using Publishy.Api.Modules.Posts.Responses;

namespace Publishy.Api.Modules.MarketingPlans.Responses;

public record MarketingPlanResponse(
    string PlanId,
    string Month,
    string[] Objectives,
    PostResponse[] Posts
);