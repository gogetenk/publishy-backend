using Publishy.Application.Common.Responses;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;

namespace Publishy.Application.UseCases.Queries.GetMarketingPlans;

public record GetMarketingPlansResponse(
    MarketingPlanResponse[] Data,
    PaginationResponse Pagination
);