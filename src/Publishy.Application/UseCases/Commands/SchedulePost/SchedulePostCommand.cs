using Ardalis.Result;
using MassTransit;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreatePost;
using MassTransit.Mediator;
using Publishy.Application.UseCases.Messages;

namespace Publishy.Application.UseCases.Commands.SchedulePost;

public record SchedulePostCommand(string PostId, DateTime ScheduledFor) : Request<Result<PostResponse>>;

public class SchedulePostCommandHandler : MediatorRequestHandler<SchedulePostCommand, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly IMessageScheduler _scheduler;

    public SchedulePostCommandHandler(
        IPostRepository postRepository,
        IMessageScheduler scheduler)
    {
        _postRepository = postRepository;
        _scheduler = scheduler;
    }

    protected override async Task<Result<PostResponse>> Handle(SchedulePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound($"Post with ID {request.PostId} was not found");

        var scheduleResult = post.Schedule(request.ScheduledFor);
        if (!scheduleResult.IsSuccess)
            return Result.Error(scheduleResult.Errors.ToArray());

        // Planifier le message pour la publication
        await _scheduler.SchedulePublish<ScheduledPostMessage>(
                DateTime.SpecifyKind(request.ScheduledFor, DateTimeKind.Utc),
                new ScheduledPostMessage(
                    post.Id,
                    post.ProjectId,
                    post.Platform,
                    request.ScheduledFor
                ),
                cancellationToken);

        await _postRepository.UpdateAsync(post, cancellationToken);
        return Result.Success((PostResponse)post);
    }
}