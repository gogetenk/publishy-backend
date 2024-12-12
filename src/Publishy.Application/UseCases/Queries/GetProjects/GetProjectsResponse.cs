using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Queries.GetProjects;

public record GetProjectsResponse(
    ProjectResponse[] Data,
    PaginationResponse Pagination
);

public record PaginationResponse(
    int CurrentPage,
    int PageSize,
    int TotalPages,
    int TotalItems
);

public record ProjectResponse(
    string Id,
    string Name,
    string Description,
    string Industry,
    string Status,
    string[] Objectives,
    TargetAudience TargetAudience,
    string BrandTone,
    string Website,
    DateTime CreatedAt,
    DateTime LastActivityDate,
    DateTime? LastScheduledPostDate,
    List<SocialMediaConfig> SocialMediaConfigs
)
{
    public static explicit operator ProjectResponse(Publishy.Application.Domain.AggregateRoots.Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.Industry,
            project.Status.ToString(),
            project.Objectives,
            project.TargetAudience,
            project.BrandTone,
            project.Website,
            project.CreatedAt,
            project.LastActivityDate,
            project.LastScheduledPostDate,
            project.SocialMediaConfigs
        );
    }
}