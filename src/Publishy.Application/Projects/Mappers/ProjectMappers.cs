using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;

namespace Publishy.Application.Projects.Mappers;

public static class ProjectMappers
{
    public static ProjectResponse MapToProjectResponse(Project project) =>
        new(
            project.Id,
            project.Name,
            project.Status.ToString(),
            project.LastActivityDate,
            project.TargetAudience.Description,
            Array.Empty<NetworkResponse>(), // To be implemented with network integration
            project.LastScheduledPostDate
        );

    public static ProjectsResponse MapToProjectsResponse(
        IEnumerable<Project> projects,
        int currentPage,
        int pageSize,
        int totalPages,
        int totalItems) =>
        new(
            projects.Select(MapToProjectResponse).ToArray(),
            new PaginationResponse(currentPage, pageSize, totalPages, totalItems)
        );
}