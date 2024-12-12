using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateProject;

namespace Publishy.Application.UseCases.Queries.GetProjects;

public record GetProjectsQuery(
    int Page,
    int PageSize,
    string? Status,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore
) : Request<Result<GetProjectsResponse>>
{
    public class GetProjectsQueryHandler : MediatorRequestHandler<GetProjectsQuery, Result<GetProjectsResponse>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectsQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        protected override async Task<Result<GetProjectsResponse>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetAllAsync(
                request.Page,
                request.PageSize,
                request.Status,
                request.CreatedAfter,
                request.CreatedBefore,
                cancellationToken
            );

            var totalItems = await _projectRepository.GetTotalCountAsync(
                request.Status,
                request.CreatedAfter,
                request.CreatedBefore,
                cancellationToken
            );

            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var projectResponses = projects.Select(project => (ProjectResponse)project).ToArray();

            var response = new GetProjectsResponse(
                Data: projectResponses,
                Pagination: new PaginationResponse(
                    CurrentPage: request.Page,
                    PageSize: request.PageSize,
                    TotalPages: totalPages,
                    TotalItems: totalItems
                )
            );

            return Result.Success(response);
        }
    }
}
