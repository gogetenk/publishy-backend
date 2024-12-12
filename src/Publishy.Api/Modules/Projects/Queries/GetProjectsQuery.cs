using MassTransit;
using Publishy.Api.Modules.Projects.Responses;

namespace Publishy.Api.Modules.Projects.Queries;

public record GetProjectsQuery(
    int Page,
    int PageSize,
    string? Status,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore
) : Request<ProjectsResponse>;