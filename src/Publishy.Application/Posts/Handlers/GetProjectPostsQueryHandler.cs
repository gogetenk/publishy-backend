using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Posts.Queries;
using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.Posts;
using Publishy.Domain.Projects;
using Publishy.Application.Posts.Mappers;

namespace Publishy.Application.Posts.Handlers;

public class GetProjectPostsQueryHandler : MediatorRequestHandler<GetProjectPostsQuery, Result<PostResponse[]>>
{
    private readonly IPostRepository _postRepository;
    private readonly IProjectRepository _projectRepository;

    public GetProjectPostsQueryHandler(
        IPostRepository postRepository,
        IProjectRepository projectRepository)
    {
        _postRepository = postRepository;
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<PostResponse[]>> Handle(GetProjectPostsQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} not found");

        var posts = await _postRepository.GetByProjectIdAsync(
            request.ProjectId,
            request.Status,
            request.MediaType,
            cancellationToken
        );

        return Result.Success(posts.Select(PostMappers.MapToPostResponse).ToArray());
    }
}
