namespace Publishy.Infrastructure.Http.Configuration;

public class OpenAIOptions
{
    public const string SectionName = "OpenAI";
    
    public string ApiKey { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4";
    public string ImageModel { get; set; } = "dall-e-3";
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
}