using MassTransit;
using Microsoft.Extensions.Logging;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Messages;

namespace Publishy.Application.UseCases.Consumers;

public class ScheduledPostConsumer : IConsumer<ScheduledPostMessage>
{
    private readonly IPostPublisher _postPublisher;
    private readonly ILogger<ScheduledPostConsumer> _logger;

    public ScheduledPostConsumer(IPostPublisher postPublisher, ILogger<ScheduledPostConsumer> logger)
    {
        _postPublisher = postPublisher;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ScheduledPostMessage> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Processing scheduled post {PostId} for project {ProjectId} on {Platform}",
            message.PostId,
            message.ProjectId,
            message.Platform);

        var result = await _postPublisher.PublishAsync(message.PostId, context.CancellationToken);
        
        if (!result.IsSuccess)
        {
            _logger.LogError(
                "Failed to publish scheduled post {PostId}: {Error}",
                message.PostId,
                string.Join(", ", result.Errors));
            
            throw new InvalidOperationException($"Failed to publish post {message.PostId}");
        }
    }
}