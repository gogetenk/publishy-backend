using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Commands;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;
using Publishy.Application.Projects.Mappers;

namespace Publishy.Application.Projects.Handlers;

public class CreateProjectCommandHandler : MediatorRequestHandler<CreateProjectCommand, Result<ProjectResponse>>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<ProjectResponse>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var targetAudience = new TargetAudience(
            request.TargetAudience.Type,
            request.TargetAudience.Description
        );

        var socialMediaConfigs = request.SocialMedias
            .Select(sm => new SocialMediaConfig(sm.Platform, sm.Frequency, sm.Timezone))
            .ToList();

        var projectResult = Project.Create(
            request.ProjectName,
            request.Description,
            request.Industry,
            request.Objectives,
            targetAudience,
            request.BrandTone,
            request.Website,
            socialMediaConfigs
        );

        if (!projectResult.IsSuccess)
            return Result.Error(projectResult.Errors);

        var project = await _projectRepository.AddAsync(projectResult.Value, cancellationToken);
        return Result.Success(ProjectMappers.MapToProjectResponse(project));
    }
}