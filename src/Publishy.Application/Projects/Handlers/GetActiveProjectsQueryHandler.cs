using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Projects.Queries;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Projects;
using Publishy.Application.Projects.Mappers;

namespace Publishy.Application.Projects.Handlers;

public class GetActiveProjectsQueryHandler : MediatorRequestHandler<GetActiveProjectsQuery, Result<ProjectResponse[]>>
{
    private readonly IProjectRepository _projectRepository;

    public GetActiveProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<ProjectResponse[]>> Handle(GetActiveProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetActiveProjectsAsync(cancellationToken);
        return Result.Success(projects.Select(ProjectMappers.MapToProjectResponse).ToArray());
    }
}