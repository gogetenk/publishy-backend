using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateProject;

namespace Publishy.Application.UseCases.Commands.UpdateProject;

public record UpdateProjectCommand(
    string ProjectId,
    string ProjectName,
    string Description,
    string Industry,
    string[] Objectives,
    TargetAudience TargetAudience,
    string BrandTone,
    string Website,
    SocialMedia[] SocialMedias
) : Request<Result<ProjectResponse>>;

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
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        var socialMediaConfigs = request.SocialMedias
            .Select(sm => new SocialMediaConfig(sm.Platform, sm.Frequency, sm.Timezone))
            .ToList();

        var updateResult = project.Update(
            request.ProjectName,
            request.Description,
            request.Industry,
            request.Objectives,
            request.TargetAudience,
            request.BrandTone,
            request.Website,
            socialMediaConfigs
        );

        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors.ToArray());

        await _projectRepository.UpdateAsync(project, cancellationToken);
        return Result.Success((ProjectResponse)project);
    }
}