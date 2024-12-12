using MassTransit;

namespace Publishy.Api.Modules.Projects.Commands;

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
) : Request<ProjectResponse>;