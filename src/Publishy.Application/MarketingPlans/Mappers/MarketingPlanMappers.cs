using Publishy.Api.Modules.MarketingPlans.Responses;
using Publishy.Domain.MarketingPlans;
using Publishy.Domain.Posts;

namespace Publishy.Application.MarketingPlans.Mappers;

public static class MarketingPlanMappers
{
    public static TimelineResponse MapToTimelineResponse(MarketingPlan plan) =>
        new(
            plan.Month,
            plan.Posts.Select(MapToTimelineEntry).ToArray()
        );

    private static TimelineEntryResponse MapToTimelineEntry(Post post) =>
        new(
            post.Id,
            post.Content,
            post.MediaType.ToString(),
            post.ScheduledDate
        );

    public static MarketingPlanResponse MapToMarketingPlanResponse(MarketingPlan plan) =>
        new(
            plan.Id,
            plan.Month,
            plan.Objectives,
            plan.Posts.Select(PostMappers.MapToPostResponse).ToArray()
        );
}