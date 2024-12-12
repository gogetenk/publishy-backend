namespace Publishy.Api.Modules.Posts.Responses;

public record PostResponse(
    string PostId,
    string ProjectId,
    string Content,
    string MediaType,
    DateTime ScheduledDate,
    string Status,
    NetworkSpecs NetworkSpecs
);