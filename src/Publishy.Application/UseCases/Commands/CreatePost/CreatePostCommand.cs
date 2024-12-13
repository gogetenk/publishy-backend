using Ardalis.Result;
using MassTransit;
using MassTransit.Mediator;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Messages;

namespace Publishy.Application.UseCases.Commands.CreatePost;

public record CreatePostCommand(
    string ProjectId,
    string Title,
    string Content,
    string Platform,
    DateTime? ScheduledFor,
    List<string> Tags,
    List<MediaAssetDto> MediaAssets
) : Request<Result<PostResponse>>;

public class CreatePostCommandHandler : MediatorRequestHandler<CreatePostCommand, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMessageScheduler _scheduler;

    public CreatePostCommandHandler(
        IPostRepository postRepository,
        IProjectRepository projectRepository,
        IMessageScheduler scheduler)
    {
        _postRepository = postRepository;
        _projectRepository = projectRepository;
        _scheduler = scheduler;
    }

    protected override async Task<Result<PostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        var mediaAssets = request.MediaAssets
            .Select(m => new MediaAsset(m.Url, m.Type, m.AltText))
            .ToList();

        var postResult = Post.Create(
            request.ProjectId,
            request.Title,
            request.Content,
            request.Platform,
            request.ScheduledFor,
            request.Tags,
            mediaAssets
        );

        if (!postResult.IsSuccess)
            return Result.Error(postResult.Errors.ToArray());

        var post = await _postRepository.AddAsync(postResult.Value, cancellationToken);

        // Si le post est programmé, envoyer le message de planification
        if (request.ScheduledFor.HasValue)
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

        return Result.Success((PostResponse)post);
    }
}