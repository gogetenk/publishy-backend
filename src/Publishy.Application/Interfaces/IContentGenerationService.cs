using Ardalis.Result;

namespace Publishy.Application.Interfaces;

public interface IContentGenerationService
{
    Task<Result<string>> GenerateTextAsync(TextGenerationRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken = default);
}

public record TextGenerationRequest(
    string Prompt,
    string? Context,
    int? MaxTokens,
    float? Temperature
);

public record ImageGenerationRequest(
    string Prompt,
    string? Style,
    string? Size,
    int? NumberOfImages
);