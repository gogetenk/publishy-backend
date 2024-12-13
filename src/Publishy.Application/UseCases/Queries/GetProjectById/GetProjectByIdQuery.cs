using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateProject;

namespace Publishy.Application.UseCases.Queries.GetProjectById;

public record GetProjectByIdQuery(string ProjectId) : Request<Result<ProjectResponse>>
{
    public class GetProjectByIdQueryHandler : MediatorRequestHandler<GetProjectByIdQuery, Result<ProjectResponse>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        protected override async Task<Result<ProjectResponse>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
            
            if (project == null)
                return Result.NotFound($"Project with ID {request.ProjectId} was not found");

            return Result.Success((ProjectResponse)project);
        }
    }
}