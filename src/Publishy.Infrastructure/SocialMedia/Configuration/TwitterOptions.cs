namespace Publishy.Infrastructure.SocialMedia.Configuration;

public class TwitterOptions
{
    public const string SectionName = "Twitter";
    
    public string ApiKey { get; set; } = string.Empty;
    public string ApiKeySecret { get; set; } = string.Empty;
    public string BearerToken { get; set; } = string.Empty;
}