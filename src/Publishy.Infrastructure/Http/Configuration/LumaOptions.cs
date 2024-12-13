namespace Publishy.Infrastructure.Http.Configuration;

public class LumaOptions
{
    public const string SectionName = "Luma";
    
    public string ApiKey { get; set; } = string.Empty;
    public string ApiEndpoint { get; set; } = "https://api.luma-api.com";
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 60;
}