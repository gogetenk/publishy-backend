using Publishy.Application.UseCases.Commands.CreateProject;

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

