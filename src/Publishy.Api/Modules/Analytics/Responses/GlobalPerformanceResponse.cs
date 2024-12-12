namespace Publishy.Api.Modules.Analytics.Responses;

public record GlobalPerformanceResponse(
    int TotalPublishedPosts,
    int TotalProjects
);