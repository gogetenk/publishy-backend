using Microsoft.Extensions.DependencyInjection;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.SocialMedia;

public class SocialMediaPublisherFactory : ISocialMediaPublisherFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SocialMediaPublisherFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISocialMediaPublisher GetPublisher(string platform)
    {
        return platform.ToLowerInvariant() switch
        {
            "twitter" => _serviceProvider.GetRequiredService<TwitterPublisher>(),
            "instagram" => _serviceProvider.GetRequiredService<InstagramPublisher>(),
            _ => throw new ArgumentException($"Unsupported platform: {platform}")
        };
    }
}