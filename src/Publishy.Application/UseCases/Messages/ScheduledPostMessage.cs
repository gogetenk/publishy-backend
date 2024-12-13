namespace Publishy.Application.UseCases.Messages;

public record ScheduledPostMessage(
    string PostId,
    string ProjectId,
    string Platform,
    DateTime ScheduledFor
);