using MassTransit;
using Publishy.Api.Modules.Posts.Responses;

namespace Publishy.Api.Modules.MarketingPlans.Queries;

public record GetMarketingPlanPostsQuery(string PlanId) : Request<PostResponse[]>;