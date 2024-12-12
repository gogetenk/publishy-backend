namespace Publishy.Api.Modules.Projects.Responses;

public record ProjectResponse(
    string ProjectId,
    string ProjectName,
    string Status,
    DateTime LastActivityDate,
    string Audience,
    NetworkResponse[] ConnectedNetworks,
    DateTime? LastScheduledPostDate
);