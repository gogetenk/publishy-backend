using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Queries.GetDashboard;

public record GetDashboardQuery() : Request<Result<DashboardResponse>>;

public class GetDashboardQueryHandler : MediatorRequestHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPostRepository _postRepository;
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetDashboardQueryHandler(
        IProjectRepository projectRepository,
        IPostRepository postRepository,
        IAnalyticsRepository analyticsRepository)
    {
        _projectRepository = projectRepository;
        _postRepository = postRepository;
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<DashboardResponse>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        // Récupérer les projets actifs
        var activeProjects = await _projectRepository.GetActiveProjectsAsync(cancellationToken);
        var projectList = activeProjects.ToList();

        // Récupérer les statistiques globales
        var overview = await GetOverviewAsync(projectList, cancellationToken);

        // Récupérer les détails des projets
        var projects = await GetProjectDetailsAsync(projectList, cancellationToken);

        return Result.Success(new DashboardResponse(overview, projects));
    }

    private async Task<DashboardOverviewResponse> GetOverviewAsync(List<Project> activeProjects, CancellationToken cancellationToken)
    {
        var lastMonth = DateTime.UtcNow.AddMonths(-1);

        // Calculer les tendances
        var previousActiveProjects = await _projectRepository.GetAllAsync(
            1, int.MaxValue, ProjectStatus.Active, lastMonth, null, cancellationToken);

        var scheduledPosts = await _postRepository.GetAllAsync(
            1, int.MaxValue, null, PostStatus.Scheduled, null, null, null, cancellationToken);

        var previousScheduledPosts = await _postRepository.GetAllAsync(
            1, int.MaxValue, null, PostStatus.Scheduled, null, lastMonth, DateTime.UtcNow, cancellationToken);

        // Calculer l'engagement total
        var totalEngagement = await CalculateTotalEngagementAsync(activeProjects.Select(p => p.Id), cancellationToken);
        var previousEngagement = await CalculateTotalEngagementAsync(
            activeProjects.Select(p => p.Id), 
            cancellationToken,
            lastMonth, 
            DateTime.UtcNow);

        var trends = new DashboardTrendInfo(
            ActiveProjectsChange: CalculatePercentageChange(previousActiveProjects.Count(), activeProjects.Count),
            EngagementChange: CalculatePercentageChange(previousEngagement, totalEngagement),
            ScheduledPostsChange: scheduledPosts.Count() - previousScheduledPosts.Count()
        );

        return new DashboardOverviewResponse(
            ActiveProjects: activeProjects.Count,
            TotalEngagement: totalEngagement,
            ScheduledPosts: scheduledPosts.Count(),
            ConnectedAccounts: activeProjects.SelectMany(p => p.SocialMediaConfigs).Select(s => s.Platform).Distinct().Count(),
            Trends: trends
        );
    }

    private async Task<DashboardProjectResponse[]> GetProjectDetailsAsync(List<Project> projects, CancellationToken cancellationToken)
    {
        var responses = new List<DashboardProjectResponse>();

        foreach (var project in projects)
        {
            var posts = await _postRepository.GetAllAsync(
                1, int.MaxValue, project.Id, null, null, null, null, cancellationToken);

            var scheduledPosts = posts.Count(p => p.Status == PostStatus.Scheduled);
            var engagement = await CalculateProjectEngagementAsync(project.Id, cancellationToken);

            responses.Add(new DashboardProjectResponse(
                Id: project.Id,
                Name: project.Name,
                Description: project.Description,
                Platforms: project.SocialMediaConfigs.Select(s => s.Platform).ToArray(),
                ScheduledPosts: scheduledPosts,
                EngagementRate: engagement,
                Metrics: new DashboardProjectMetrics(
                    TotalPosts: posts.Count(),
                    Interactions: posts.Count(p => p.Status == PostStatus.Published),
                    AverageEngagement: engagement
                )
            ));
        }

        return responses.ToArray();
    }

    private async Task<decimal> CalculateTotalEngagementAsync(
        IEnumerable<string> projectIds, 
        CancellationToken cancellationToken,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var totalEngagement = 0m;

        foreach (var projectId in projectIds)
        {
            totalEngagement += await CalculateProjectEngagementAsync(projectId, cancellationToken, startDate, endDate);
        }

        return totalEngagement;
    }

    private async Task<decimal> CalculateProjectEngagementAsync(
        string projectId, 
        CancellationToken cancellationToken,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var analytics = await _analyticsRepository.GetAllAsync(
            1, int.MaxValue, projectId, null, startDate, endDate, cancellationToken);

        if (!analytics.Any())
            return 0;

        return analytics
            .SelectMany(a => a.Metrics)
            .Where(m => m.Category == "engagement")
            .Average(m => m.Value);
    }

    private static decimal CalculatePercentageChange(decimal previous, decimal current)
    {
        if (previous == 0)
            return current > 0 ? 100 : 0;

        return ((current - previous) / previous) * 100;
    }
}