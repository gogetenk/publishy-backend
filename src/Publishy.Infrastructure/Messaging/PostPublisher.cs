using Ardalis.Result;
using Microsoft.Extensions.Logging;
using Polly;
using Publishy.Application.Domain.Entities;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.Messaging;

public class PostPublisher : IPostPublisher
{
    private readonly IPostRepository _postRepository;
    private readonly IPublicationAttemptRepository _attemptRepository;
    private readonly ISocialMediaPublisherFactory _publisherFactory;
    private readonly ILogger<PostPublisher> _logger;

    private const int MaxRetries = 3;
    private static readonly TimeSpan RetryWindow = TimeSpan.FromHours(1);

    public PostPublisher(
        IPostRepository postRepository,
        IPublicationAttemptRepository attemptRepository,
        ISocialMediaPublisherFactory publisherFactory,
        ILogger<PostPublisher> logger)
    {
        _postRepository = postRepository;
        _attemptRepository = attemptRepository;
        _publisherFactory = publisherFactory;
        _logger = logger;
    }

    public async Task<Result> PublishAsync(string postId, CancellationToken cancellationToken = default)
    {
        try
        {
            var post = await _postRepository.GetByIdAsync(postId, cancellationToken);
            if (post == null)
                return Result.NotFound($"Post with ID {postId} was not found");

            // Vérifier le nombre de tentatives échouées récentes
            var recentFailures = await _attemptRepository.GetFailedAttemptsCountAsync(postId, RetryWindow, cancellationToken);
            if (recentFailures >= MaxRetries)
            {
                var error = $"Maximum retry attempts ({MaxRetries}) reached for post {postId}";
                _logger.LogError(error);
                await TrackAttemptAsync(postId, post.Platform, false, error, recentFailures, cancellationToken);
                return Result.Error(error);
            }

            // Obtenir le publisher approprié pour la plateforme
            var publisher = _publisherFactory.GetPublisher(post.Platform);

            // Créer une politique de retry
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    MaxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: async (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            exception,
                            "Retry {RetryCount} publishing post {PostId} after {Delay}s",
                            retryCount,
                            postId,
                            timeSpan.TotalSeconds);

                        await TrackAttemptAsync(postId, post.Platform, false, exception.Message, retryCount, cancellationToken);
                    }
                );

            // Exécuter la publication avec retry
            var publishResult = await retryPolicy.ExecuteAsync(async () =>
            {
                var result = await publisher.PublishAsync(new SocialMediaPost(
                    Content: post.Content,
                    MediaUrls: post.MediaAssets.Select(m => m.Url).ToList(),
                    Metadata: new Dictionary<string, string>
                    {
                        { "title", post.Title },
                        { "tags", string.Join(",", post.Tags) }
                    }
                ), cancellationToken);

                if (!result.IsSuccess)
                    throw new Exception(string.Join(", ", result.Errors));

                return result;
            });

            // Enregistrer la tentative réussie
            await TrackAttemptAsync(postId, post.Platform, true, null, recentFailures, cancellationToken);

            // Mettre à jour le statut du post
            var updateResult = post.Publish();
            if (!updateResult.IsSuccess)
                return Result.Error(updateResult.Errors.ToArray());

            await _postRepository.UpdateAsync(post, cancellationToken);
            
            _logger.LogInformation("Post {PostId} published successfully to {Platform}", postId, post.Platform);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing post {PostId}", postId);
            return Result.Error($"Failed to publish post: {ex.Message}");
        }
    }

    private async Task TrackAttemptAsync(
        string postId,
        string platform,
        bool succeeded,
        string? errorMessage,
        int retryCount,
        CancellationToken cancellationToken)
    {
        var attempt = new PublicationAttempt(postId, platform, succeeded, errorMessage, retryCount);
        await _attemptRepository.AddAsync(attempt, cancellationToken);
    }
}
