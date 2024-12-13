using System.Net.Http.Json;
using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publishy.Application.Interfaces;
using Publishy.Infrastructure.Http.Configuration;

namespace Publishy.Infrastructure.Http;

public class LumaVideoService : IVideoGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LumaVideoService> _logger;
    private readonly LumaOptions _options;

    public LumaVideoService(
        HttpClient httpClient,
        IOptions<LumaOptions> options,
        ILogger<LumaVideoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        
        _httpClient.BaseAddress = new Uri(_options.ApiEndpoint);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
    }

    public async Task<Result<string>> GenerateVideoAsync(VideoGenerationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("v1/videos/generations", new
            {
                prompt = request.Prompt,
                style = request.Style,
                aspect_ratio = request.AspectRatio ?? "16:9",
                duration_seconds = request.DurationInSeconds ?? 15,
                with_audio = request.WithAudio ?? false
            }, cancellationToken);

            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<VideoGenerationResponse>(cancellationToken: cancellationToken);
            return Result.Success(result?.Id ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating video with Luma");
            return Result.Error("Failed to generate video: " + ex.Message);
        }
    }

    public async Task<Result<string>> GetVideoStatusAsync(string videoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"v1/videos/{videoId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<VideoStatusResponse>(cancellationToken: cancellationToken);
            return Result.Success(result?.Status ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting video status from Luma");
            return Result.Error("Failed to get video status: " + ex.Message);
        }
    }

    private record VideoGenerationResponse(string Id);
    private record VideoStatusResponse(string Status, string? Url);
}