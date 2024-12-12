namespace Publishy.Api.Modules.MarketingPlans.Responses;

public record TimelineResponse(
    string Month,
    TimelineEntryResponse[] Entries
);

public record TimelineEntryResponse(
    string PostId,
    string Title,
    string MediaType,
    DateTime ScheduledTime
);