using Ardalis.Result;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.Domain.AggregateRoots;

public class Project
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Industry { get; private set; }
    public ProjectStatus Status { get; private set; }
    public string[] Objectives { get; private set; }
    public TargetAudience TargetAudience { get; private set; }
    public string BrandTone { get; private set; }
    public string Website { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActivityDate { get; private set; }
    public DateTime? LastScheduledPostDate { get; private set; }
    public List<SocialMediaConfig> SocialMediaConfigs { get; private set; }

    private Project() { } // For EF Core

    private Project(
        string name,
        string description,
        string industry,
        string[] objectives,
        TargetAudience targetAudience,
        string brandTone,
        string website,
        List<SocialMediaConfig> socialMediaConfigs)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
        Industry = industry;
        Status = ProjectStatus.Active;
        Objectives = objectives;
        TargetAudience = targetAudience;
        BrandTone = brandTone;
        Website = website;
        CreatedAt = DateTime.UtcNow;
        LastActivityDate = DateTime.UtcNow;
        SocialMediaConfigs = socialMediaConfigs;
    }

    public static Result<Project> Create(
        string name,
        string description,
        string industry,
        string[] objectives,
        TargetAudience targetAudience,
        string brandTone,
        string website,
        List<SocialMediaConfig> socialMediaConfigs)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Project name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Project description cannot be empty");

        if (string.IsNullOrWhiteSpace(industry))
            return Result.Error("Project industry cannot be empty");

        if (objectives == null || !objectives.Any())
            return Result.Error("Project must have at least one objective");

        if (targetAudience == null)
            return Result.Error("Project must have a target audience");

        if (string.IsNullOrWhiteSpace(website))
            return Result.Error("Project website cannot be empty");

        if (!Uri.TryCreate(website, UriKind.Absolute, out _))
            return Result.Error("Project website must be a valid URL");

        if (socialMediaConfigs == null || !socialMediaConfigs.Any())
            return Result.Error("Project must have at least one social media configuration");

        return Result.Success(new Project(name, description, industry, objectives, targetAudience, brandTone, website, socialMediaConfigs));
    }

    public Result UpdateStatus(bool isActive)
    {
        if (Status == ProjectStatus.Active && !isActive)
        {
            Status = ProjectStatus.Inactive;
            LastActivityDate = DateTime.UtcNow;
            return Result.Success();
        }

        if (Status == ProjectStatus.Inactive && isActive)
        {
            Status = ProjectStatus.Active;
            LastActivityDate = DateTime.UtcNow;
            return Result.Success();
        }

        return Result.Error("Project status is already in the requested state");
    }

    public Result Update(
        string name,
        string description,
        string industry,
        string[] objectives,
        TargetAudience targetAudience,
        string brandTone,
        string website,
        List<SocialMediaConfig> socialMediaConfigs)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Project name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Project description cannot be empty");

        if (string.IsNullOrWhiteSpace(industry))
            return Result.Error("Project industry cannot be empty");

        if (objectives == null || !objectives.Any())
            return Result.Error("Project must have at least one objective");

        if (targetAudience == null)
            return Result.Error("Project must have a target audience");

        if (string.IsNullOrWhiteSpace(website))
            return Result.Error("Project website cannot be empty");

        if (!Uri.TryCreate(website, UriKind.Absolute, out _))
            return Result.Error("Project website must be a valid URL");

        if (socialMediaConfigs == null || !socialMediaConfigs.Any())
            return Result.Error("Project must have at least one social media configuration");

        Name = name;
        Description = description;
        Industry = industry;
        Objectives = objectives;
        TargetAudience = targetAudience;
        BrandTone = brandTone;
        Website = website;
        SocialMediaConfigs = socialMediaConfigs;
        LastActivityDate = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateLastScheduledPostDate(DateTime date)
    {
        if (date < DateTime.UtcNow)
            return Result.Error("Last scheduled post date cannot be in the past");

        LastScheduledPostDate = date;
        LastActivityDate = DateTime.UtcNow;
        return Result.Success();
    }
}

public enum ProjectStatus
{
    Active,
    Inactive
}