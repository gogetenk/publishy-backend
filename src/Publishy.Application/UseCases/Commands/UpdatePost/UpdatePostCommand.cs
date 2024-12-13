using Ardalis.Result;
using MassTransit;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.UseCases.Messages;
using MassTransit.Mediator;

namespace Publishy.Application.UseCases.Commands.UpdatePost;

public record UpdatePostCommand(
    string PostId,
    string Title,
    string Content,
    string Platform,
    DateTime? ScheduledFor,
    List<string> Tags,
    List<MediaAssetDto> MediaAssets
) : Request<Result<PostResponse>>;

public class UpdatePostCommandHandler : MediatorRequestHandler<UpdatePostCommand, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly IMessageScheduler _scheduler;

    public UpdatePostCommandHandler(
        IPostRepository postRepository,
        IMessageScheduler scheduler)
    {
        _postRepository = postRepository;
        _scheduler = scheduler;
    }

    protected override async Task<Result<PostResponse>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null)
            return Result.NotFound($"Post with ID {request.PostId} was not found");

        var mediaAssets = request.MediaAssets
            .Select(m => new MediaAsset(m.Url, m.Type, m.AltText))
            .ToList();

        var updateResult = post.Update(
            request.Title,
            request.Content,
            request.Platform,
            request.ScheduledFor,
            request.Tags,
            mediaAssets
        );

        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors.ToArray());

        // Si le post est programmé, envoyer le message de planification
        if (request.ScheduledFor.HasValue && post.Status != Domain.AggregateRoots.PostStatus.Published)
        {
            await _scheduler.SchedulePublish<ScheduledPostMessage>(
                DateTime.SpecifyKind(request.ScheduledFor.Value, DateTimeKind.Utc),
                new ScheduledPostMessage(
                    post.Id,
                    post.ProjectId,
                    post.Platform,
                    request.ScheduledFor.Value
                ),
                cancellationToken);
        }

        await _postRepository.UpdateAsync(post, cancellationToken);
        return Result.Success((PostResponse)post);
    }
}