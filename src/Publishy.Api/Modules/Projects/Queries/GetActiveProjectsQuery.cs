using MassTransit;
using Publishy.Api.Modules.Projects.Responses;

namespace Publishy.Api.Modules.Projects.Queries;

public record GetActiveProjectsQuery() : Request<ProjectResponse[]>;