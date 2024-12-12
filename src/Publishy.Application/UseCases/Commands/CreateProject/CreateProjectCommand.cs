using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.CreateProject;

public record CreateProjectCommand(
    string ProjectName,
    string Description,
    string Industry,
    string[] Objectives,
    TargetAudience TargetAudience,
    string BrandTone,
    string Website,
    SocialMedia[] SocialMedias
) : Request<Result<Project>>;

public record SocialMedia(string Platform, int Frequency, string Timezone);

public class CreateProjectCommandHandler : MediatorRequestHandler<CreateProjectCommand, Result<Project>>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<Project>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
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
            return projectResult;

        var project = await _projectRepository.AddAsync(projectResult.Value, cancellationToken);
        return Result.Success(project);
    }
}
