using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publishy.Application.Interfaces;
using Publishy.Infrastructure.SocialMedia.Configuration;
using System.Net.Http.Json;

namespace Publishy.Infrastructure.SocialMedia;

public class InstagramPublisher : ISocialMediaPublisher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InstagramPublisher> _logger;
    private readonly InstagramOptions _options;

    public InstagramPublisher(
        HttpClient httpClient,
        IOptions<InstagramOptions> options,
        ILogger<InstagramPublisher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri("https://graph.instagram.com/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.AccessToken}");
    }

    public async Task<Result> PublishAsync(SocialMediaPost post, CancellationToken cancellationToken = default)
    {
        try
        {
            // Pour Instagram, on doit d'abord créer un container média
            var containerId = await CreateMediaContainerAsync(post, cancellationToken);
            if (string.IsNullOrEmpty(containerId))
            {
                return Result.Error("Failed to create media container");
            }

            // Publier le post
            var response = await _httpClient.PostAsJsonAsync($"me/media_publish", new
            {
                creation_id = containerId
            }, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Instagram API error: {Error}", error);
                return Result.Error($"Failed to publish post: {error}");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to Instagram");
            return Result.Error($"Failed to publish to Instagram: {ex.Message}");
        }
    }

    private async Task<string?> CreateMediaContainerAsync(SocialMediaPost post, CancellationToken cancellationToken)
    {
        try
        {
            // Pour Instagram, on ne peut publier qu'une seule image/vidéo à la fois
            var mediaUrl = post.MediaUrls.FirstOrDefault();
            if (string.IsNullOrEmpty(mediaUrl))
            {
                _logger.LogError("Instagram requires at least one media");
                return null;
            }

            var response = await _httpClient.PostAsJsonAsync("me/media", new
            {
                image_url = mediaUrl,
                caption = post.Content
            }, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to create Instagram media container");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<InstagramMediaResponse>(cancellationToken: cancellationToken);
            return result?.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Instagram media container");
            return null;
        }
    }

    private record InstagramMediaResponse(string Id);
}