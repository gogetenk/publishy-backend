namespace Publishy.Api.Modules.Projects.Responses;

public record ProjectsResponse(
    ProjectResponse[] Data,
    PaginationResponse Pagination
);

public record PaginationResponse(
    int CurrentPage,
    int PageSize,
    int TotalPages,
    int TotalItems
);