using MassTransit;

namespace Publishy.Api.Modules.Projects.Commands;

public record CreateProjectCommand(
    string ProjectName,
    string Description,
    string Industry,
    string[] Objectives,
    TargetAudience TargetAudience,
    string BrandTone,
    string Website,
    SocialMedia[] SocialMedias
) : Request<ProjectResponse>;

public record TargetAudience(string Type, string Description);

public record SocialMedia(string Platform, int Frequency, string Timezone);