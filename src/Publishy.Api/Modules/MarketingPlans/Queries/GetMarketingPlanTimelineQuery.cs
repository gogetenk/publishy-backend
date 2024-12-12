using MassTransit;
using Publishy.Api.Modules.MarketingPlans.Responses;

namespace Publishy.Api.Modules.MarketingPlans.Queries;

public record GetMarketingPlanTimelineQuery(string PlanId) : Request<TimelineResponse>;