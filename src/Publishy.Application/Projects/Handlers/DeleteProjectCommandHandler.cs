using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Commands;
using Publishy.Domain.Projects;

namespace Publishy.Application.Projects.Handlers;

public class DeleteProjectCommandHandler : MediatorRequestHandler<DeleteProjectCommand, Result>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} not found");

        await _projectRepository.DeleteAsync(request.ProjectId, cancellationToken);
        return Result.Success();
    }
}