using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Queries;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;
using Publishy.Application.Projects.Mappers;

namespace Publishy.Application.Projects.Handlers;

public class GetProjectByIdQueryHandler : MediatorRequestHandler<GetProjectByIdQuery, Result<ProjectResponse>>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<ProjectResponse>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} not found");

        return Result.Success(ProjectMappers.MapToProjectResponse(project));
    }
}