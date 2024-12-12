using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Commands;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;
using Publishy.Application.Projects.Mappers;

namespace Publishy.Application.Projects.Handlers;

public class UpdateProjectCommandHandler : MediatorRequestHandler<UpdateProjectCommand, Result<ProjectResponse>>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<ProjectResponse>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} not found");

        var targetAudience = new TargetAudience(
            request.TargetAudience.Type,
            request.TargetAudience.Description
        );

        var socialMediaConfigs = request.SocialMedias
            .Select(sm => new SocialMediaConfig(sm.Platform, sm.Frequency, sm.Timezone))
            .ToList();

        var updateResult = project.Update(
            request.ProjectName,
            request.Description,
            request.Industry,
            request.Objectives,
            targetAudience,
            request.BrandTone,
            request.Website,
            socialMediaConfigs
        );

        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors);

        await _projectRepository.UpdateAsync(project, cancellationToken);
        return Result.Success(ProjectMappers.MapToProjectResponse(project));
    }
}