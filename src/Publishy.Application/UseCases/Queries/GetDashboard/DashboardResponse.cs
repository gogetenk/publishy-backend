using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Queries.GetDashboard;

public record DashboardResponse(
    DashboardOverviewResponse Overview,
    DashboardProjectResponse[] Projects
);

public record DashboardOverviewResponse(
    int ActiveProjects,
    decimal TotalEngagement,
    int ScheduledPosts,
    int ConnectedAccounts,
    DashboardTrendInfo Trends
);

public record DashboardProjectResponse(
    string Id,
    string Name,
    string Description,
    string[] Platforms,
    int ScheduledPosts,
    decimal EngagementRate,
    DashboardProjectMetrics Metrics
);

public record DashboardTrendInfo(
    decimal ActiveProjectsChange,
    decimal EngagementChange,
    int ScheduledPostsChange
);

public record DashboardProjectMetrics(
    int TotalPosts,
    int Interactions,
    decimal AverageEngagement
);