using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Commands;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;
using Publishy.Application.Projects.Mappers;

namespace Publishy.Application.Projects.Handlers;

public class UpdateProjectStatusCommandHandler : MediatorRequestHandler<UpdateProjectStatusCommand, Result<ProjectResponse>>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectStatusCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<ProjectResponse>> Handle(UpdateProjectStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} not found");

        var updateResult = project.UpdateStatus(request.IsActive);
        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors);

        await _projectRepository.UpdateAsync(project, cancellationToken);
        return Result.Success(ProjectMappers.MapToProjectResponse(project));
    }
}