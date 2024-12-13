using System.Net.Http.Json;
using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publishy.Application.Interfaces;
using Publishy.Infrastructure.Http.Configuration;

namespace Publishy.Infrastructure.Http;

public class OpenAIContentService : IContentGenerationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIContentService> _logger;
    private readonly OpenAIOptions _options;

    public OpenAIContentService(
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger<OpenAIContentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ApiKey}");
        if (!string.IsNullOrEmpty(_options.Organization))
        {
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", _options.Organization);
        }
    }

    public async Task<Result<string>> GenerateTextAsync(TextGenerationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("v1/chat/completions", new
            {
                model = _options.Model,
                messages = new[]
                {
                    new { role = "system", content = request.Context ?? "You are a helpful assistant." },
                    new { role = "user", content = request.Prompt }
                },
                max_tokens = request.MaxTokens,
                temperature = request.Temperature
            }, cancellationToken);

            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OpenAITextResponse>(cancellationToken: cancellationToken);
            return Result.Success(result?.Choices[0].Message.Content ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text with OpenAI");
            return Result.Error("Failed to generate text: " + ex.Message);
        }
    }

    public async Task<Result<string>> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("v1/images/generations", new
            {
                model = _options.ImageModel,
                prompt = request.Prompt,
                size = request.Size ?? "1024x1024",
                n = request.NumberOfImages ?? 1,
                style = request.Style
            }, cancellationToken);

            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OpenAIImageResponse>(cancellationToken: cancellationToken);
            return Result.Success(result?.Data[0].Url ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating image with OpenAI");
            return Result.Error("Failed to generate image: " + ex.Message);
        }
    }

    private record OpenAITextResponse(Choice[] Choices);
    private record Choice(Message Message);
    private record Message(string Content);
    
    private record OpenAIImageResponse(ImageData[] Data);
    private record ImageData(string Url);
}