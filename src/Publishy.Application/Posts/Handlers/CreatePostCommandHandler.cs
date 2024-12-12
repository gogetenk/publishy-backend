using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Posts.Commands;
using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.Posts;
using Publishy.Domain.Projects;
using Publishy.Application.Posts.Mappers;

namespace Publishy.Application.Posts.Handlers;

public class CreatePostCommandHandler : MediatorRequestHandler<CreatePostCommand, Result<PostResponse>>
{
    private readonly IPostRepository _postRepository;
    private readonly IProjectRepository _projectRepository;

    public CreatePostCommandHandler(
        IPostRepository postRepository,
        IProjectRepository projectRepository)
    {
        _postRepository = postRepository;
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<PostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} not found");

        var networkSpecs = new NetworkSpecifications
        {
            Twitter = request.NetworkSpecs.Twitter != null
                ? new TwitterSpecifications(request.NetworkSpecs.Twitter.TweetLength)
                : null,
            LinkedIn = request.NetworkSpecs.LinkedIn != null
                ? new LinkedInSpecifications(request.NetworkSpecs.LinkedIn.PostType)
                : null,
            Instagram = request.NetworkSpecs.Instagram != null
                ? new InstagramSpecifications(request.NetworkSpecs.Instagram.ImageDimensions)
                : null,
            Blog = request.NetworkSpecs.Blog != null
                ? new BlogSpecifications(request.NetworkSpecs.Blog.Category)
                : null,
            Newsletter = request.NetworkSpecs.Newsletter != null
                ? new NewsletterSpecifications(request.NetworkSpecs.Newsletter.SubjectLine)
                : null
        };

        var postResult = Post.Create(
            request.ProjectId,
            request.Content,
            Enum.Parse<MediaType>(request.MediaType),
            request.ScheduledDate,
            networkSpecs
        );

        if (!postResult.IsSuccess)
            return Result.Error(postResult.Errors);

        var post = await _postRepository.AddAsync(postResult.Value, cancellationToken);

        // Update project's last scheduled post date
        var updateResult = project.UpdateLastScheduledPostDate(post.ScheduledDate);
        if (updateResult.IsSuccess)
            await _projectRepository.UpdateAsync(project, cancellationToken);

        return Result.Success(PostMappers.MapToPostResponse(post));
    }
}
