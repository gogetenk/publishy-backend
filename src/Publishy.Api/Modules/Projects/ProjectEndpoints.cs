using Ardalis.Result;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Publishy.Api.Modules.Projects.Commands;
using Publishy.Api.Modules.Projects.Queries;
using Publishy.Api.Modules.Projects.Responses;
using Publishy.Domain.Common.Results;

namespace Publishy.Api.Modules.Projects;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects")
            .WithTags("Projects")
            .WithOpenApi();

        // GET /projects
        group.MapGet("/", async (IMediator mediator, int? page, int? pageSize, string? status, DateTime? createdAfter, DateTime? createdBefore) =>
        {
            var query = new GetProjectsQuery(page ?? 1, pageSize ?? 10, status, createdAfter, createdBefore);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetProjects")
        .WithSummary("Retrieve the list of projects")
        .WithDescription("Fetches all projects with their details.")
        .Produces<Result<ProjectsResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // POST /projects
        group.MapPost("/", async (IMediator mediator, CreateProjectCommand command) =>
        {
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("CreateProject")
        .WithSummary("Create a new project")
        .WithDescription("Creates a new project with the provided information.")
        .Produces<Result<ProjectResponse>>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /projects/active
        group.MapGet("/active", async (IMediator mediator) =>
        {
            var query = new GetActiveProjectsQuery();
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetActiveProjects")
        .WithSummary("Retrieve the list of active projects")
        .WithDescription("Fetches only active projects.")
        .Produces<Result<ProjectResponse[]>>()
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // GET /projects/{projectId}
        group.MapGet("/{projectId}", async (IMediator mediator, string projectId) =>
        {
            var query = new GetProjectByIdQuery(projectId);
            var response = await mediator.SendRequest(query);
            return response.ToMinimalApiResult();
        })
        .WithName("GetProjectById")
        .WithSummary("Retrieve project details")
        .WithDescription("Fetches details of a specific project.")
        .Produces<Result<ProjectResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /projects/{projectId}
        group.MapPut("/{projectId}", async (IMediator mediator, string projectId, UpdateProjectCommand command) =>
        {
            command = command with { ProjectId = projectId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdateProject")
        .WithSummary("Update a project")
        .WithDescription("Updates information of an existing project.")
        .Produces<Result<ProjectResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // DELETE /projects/{projectId}
        group.MapDelete("/{projectId}", async (IMediator mediator, string projectId) =>
        {
            var command = new DeleteProjectCommand(projectId);
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("DeleteProject")
        .WithSummary("Delete a project")
        .WithDescription("Deletes an existing project.")
        .Produces<Result>(StatusCodes.Status204NoContent)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        // PUT /projects/{projectId}/status
        group.MapPut("/{projectId}/status", async (IMediator mediator, string projectId, UpdateProjectStatusCommand command) =>
        {
            command = command with { ProjectId = projectId };
            var response = await mediator.SendRequest(command);
            return response.ToMinimalApiResult();
        })
        .WithName("UpdateProjectStatus")
        .WithSummary("Activate or deactivate a project")
        .WithDescription("Updates the status of a project.")
        .Produces<Result<ProjectResponse>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

        return app;
    }
}