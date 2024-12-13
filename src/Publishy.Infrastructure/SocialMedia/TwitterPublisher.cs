using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publishy.Application.Interfaces;
using Publishy.Infrastructure.SocialMedia.Configuration;
using System.Net.Http.Json;

namespace Publishy.Infrastructure.SocialMedia;

public class TwitterPublisher : ISocialMediaPublisher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TwitterPublisher> _logger;
    private readonly TwitterOptions _options;

    public TwitterPublisher(
        HttpClient httpClient,
        IOptions<TwitterOptions> options,
        ILogger<TwitterPublisher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri("https://api.twitter.com/2/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.BearerToken}");
    }

    public async Task<Result> PublishAsync(SocialMediaPost post, CancellationToken cancellationToken = default)
    {
        try
        {
            // Publier d'abord les médias si présents
            var mediaIds = new List<string>();
            foreach (var mediaUrl in post.MediaUrls)
            {
                var mediaId = await UploadMediaAsync(mediaUrl, cancellationToken);
                if (mediaId != null)
                {
                    mediaIds.Add(mediaId);
                }
            }

            // Créer le tweet
            var response = await _httpClient.PostAsJsonAsync("tweets", new
            {
                text = post.Content,
                media = mediaIds.Any() ? new { media_ids = mediaIds } : null
            }, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Twitter API error: {Error}", error);
                return Result.Error($"Failed to publish tweet: {error}");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to Twitter");
            return Result.Error($"Failed to publish to Twitter: {ex.Message}");
        }
    }

    private async Task<string?> UploadMediaAsync(string mediaUrl, CancellationToken cancellationToken)
    {
        try
        {
            var mediaBytes = await _httpClient.GetByteArrayAsync(mediaUrl, cancellationToken);
            var mediaContent = new ByteArrayContent(mediaBytes);
            
            var response = await _httpClient.PostAsync("media/upload", mediaContent, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to upload media to Twitter");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TwitterMediaResponse>(cancellationToken: cancellationToken);
            return result?.MediaId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading media to Twitter");
            return null;
        }
    }

    private record TwitterMediaResponse(string MediaId);
}