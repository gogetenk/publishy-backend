using MassTransit;
using Publishy.Api.Modules.MarketingPlans.Responses;

namespace Publishy.Api.Modules.MarketingPlans.Commands;

public record AddPostToTimelineCommand(
    string PlanId,
    string Content,
    string MediaType,
    DateTime ScheduledDate
) : Request<TimelineEntryResponse>;