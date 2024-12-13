//using MassTransit;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Publishy.Application.Interfaces;
//using Publishy.Application.UseCases.Messages;

//namespace Publishy.Application.UseCases.BackgroundServices;

//public class ScheduledPostsProcessor : BackgroundService
//{
//    private readonly IPostRepository _postRepository;
//    private readonly IPublishEndpoint _publishEndpoint;
//    private readonly ILogger<ScheduledPostsProcessor> _logger;
//    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

//    public ScheduledPostsProcessor(
//        IPostRepository postRepository,
//        IPublishEndpoint publishEndpoint,
//        ILogger<ScheduledPostsProcessor> logger)
//    {
//        _postRepository = postRepository;
//        _publishEndpoint = publishEndpoint;
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                var now = DateTime.UtcNow;
//                var scheduledPosts = await _postRepository.GetScheduledPostsAsync(now, stoppingToken);

//                foreach (var post in scheduledPosts)
//                {
//                    await _publishEndpoint.Publish(new ScheduledPostMessage(
//                        post.Id,
//                        post.ProjectId,
//                        post.Platform,
//                        post.ScheduledFor!.Value
//                    ), stoppingToken);

//                    _logger.LogInformation(
//                        "Scheduled post {PostId} queued for publishing at {ScheduledTime}",
//                        post.Id,
//                        post.ScheduledFor);
//                }

//                await Task.Delay(_checkInterval, stoppingToken);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing scheduled posts");
//                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
//            }
//        }
//    }
//}