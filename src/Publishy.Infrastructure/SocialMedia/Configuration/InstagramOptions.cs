namespace Publishy.Infrastructure.SocialMedia.Configuration;

public class InstagramOptions
{
    public const string SectionName = "Instagram";
    
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}