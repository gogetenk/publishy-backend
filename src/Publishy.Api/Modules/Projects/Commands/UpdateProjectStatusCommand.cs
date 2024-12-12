using MassTransit;

namespace Publishy.Api.Modules.Projects.Commands;

public record UpdateProjectStatusCommand(string ProjectId, bool IsActive) : Request<ProjectResponse>;