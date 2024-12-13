using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateProject;

namespace Publishy.Application.UseCases.Queries.GetActiveProjects;

public record GetActiveProjectsQuery() : Request<Result<ProjectResponse[]>>
{
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
            var projectResponses = projects.Select(project => (ProjectResponse)project).ToArray();
            return Result.Success(projectResponses);
        }
    }
}