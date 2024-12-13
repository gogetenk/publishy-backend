using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateProject;

namespace Publishy.Application.UseCases.Commands.UpdateProjectStatus;

public record UpdateProjectStatusCommand(string ProjectId, bool IsActive) : Request<Result<ProjectResponse>>;

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
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        var updateResult = project.UpdateStatus(request.IsActive);
        
        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors.ToArray());

        await _projectRepository.UpdateAsync(project, cancellationToken);
        return Result.Success((ProjectResponse)project);
    }
}