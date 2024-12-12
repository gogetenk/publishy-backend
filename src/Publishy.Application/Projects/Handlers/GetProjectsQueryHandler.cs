using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Queries;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;
using Publishy.Application.Projects.Mappers;

namespace Publishy.Application.Projects.Handlers;

public class GetProjectsQueryHandler : MediatorRequestHandler<GetProjectsQuery, Result<ProjectsResponse>>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<ProjectsResponse>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.Status,
            request.CreatedAfter,
            request.CreatedBefore,
            cancellationToken
        );

        var totalItems = await _projectRepository.GetTotalCountAsync(
            request.Status,
            request.CreatedAfter,
            request.CreatedBefore,
            cancellationToken
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        return Result.Success(ProjectMappers.MapToProjectsResponse(
            projects,
            request.Page,
            request.PageSize,
            totalPages,
            totalItems
        ));
    }
}