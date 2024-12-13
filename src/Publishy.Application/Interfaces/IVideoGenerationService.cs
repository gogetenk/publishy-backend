using Ardalis.Result;

namespace Publishy.Application.Interfaces;

public interface IVideoGenerationService
{
    Task<Result<string>> GenerateVideoAsync(VideoGenerationRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> GetVideoStatusAsync(string videoId, CancellationToken cancellationToken = default);
}

public record VideoGenerationRequest(
    string Prompt,
    string? Style,
    string? AspectRatio,
    int? DurationInSeconds,
    bool? WithAudio
);