namespace Publishy.Application.Domain.ValueObjects;

public record SocialMediaConfig
{
    public string Platform { get; init; }
    public int Frequency { get; init; }
    public string Timezone { get; init; }

    public SocialMediaConfig(string platform, int frequency, string timezone)
    {
        Platform = platform;
        Frequency = frequency;
        Timezone = timezone;
    }
}