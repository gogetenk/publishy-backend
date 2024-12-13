using Publishy.Application.UseCases.Commands.CreateProject;
using Publishy.Application.Common.Responses;

namespace Publishy.Application.UseCases.Queries.GetProjects;

public record GetProjectsResponse(
    ProjectResponse[] Data,
    PaginationResponse Pagination
);

