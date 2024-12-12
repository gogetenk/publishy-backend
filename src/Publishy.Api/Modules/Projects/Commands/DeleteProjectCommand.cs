using MassTransit;

namespace Publishy.Api.Modules.Projects.Commands;

public record DeleteProjectCommand(string ProjectId) : Request;