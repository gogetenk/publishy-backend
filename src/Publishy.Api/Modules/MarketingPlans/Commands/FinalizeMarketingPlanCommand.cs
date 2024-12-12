using MassTransit;
using Publishy.Api.Modules.MarketingPlans.Responses;

namespace Publishy.Api.Modules.MarketingPlans.Commands;

public record FinalizeMarketingPlanCommand(string PlanId) : Request<MarketingPlanResponse>;