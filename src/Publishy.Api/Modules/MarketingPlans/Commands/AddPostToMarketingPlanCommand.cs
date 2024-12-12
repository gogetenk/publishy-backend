using MassTransit;
using Publishy.Api.Modules.Posts.Responses;

namespace Publishy.Api.Modules.MarketingPlans.Commands;

public record AddPostToMarketingPlanCommand(
    string PlanId,
    string Content,
    string MediaType,
    DateTime ScheduledDate
) : Request<PostResponse>;