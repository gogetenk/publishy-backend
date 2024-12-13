using Ardalis.Result;
using MassTransit;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Messages;
using MassTransit.Mediator;

namespace Publishy.Application.UseCases.Commands.DeleteProject;

public record DeleteProjectCommand(string ProjectId) : Request<Result>;

public class DeleteProjectCommandHandler : MediatorRequestHandler<DeleteProjectCommand, Result>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IPostRepository _postRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        IPostRepository postRepository,
        IPublishEndpoint publishEndpoint)
    {
        _projectRepository = projectRepository;
        _postRepository = postRepository;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        // Récupérer tous les posts du projet
        var posts = await _postRepository.GetAllAsync(
            page: 1,
            pageSize: int.MaxValue,
            projectId: request.ProjectId,
            cancellationToken: cancellationToken
        );

        // Annuler tous les posts programmés
        foreach (var post in posts.Where(p => p.Status == Domain.AggregateRoots.PostStatus.Scheduled))
        {
            await _publishEndpoint.Publish(new CancelScheduledPostMessage(post.Id), cancellationToken);
        }

        // Supprimer tous les posts
        foreach (var post in posts)
        {
            await _postRepository.DeleteAsync(post.Id, cancellationToken);
        }

        // Supprimer le projet
        await _projectRepository.DeleteAsync(request.ProjectId, cancellationToken);
        return Result.Success();
    }
}