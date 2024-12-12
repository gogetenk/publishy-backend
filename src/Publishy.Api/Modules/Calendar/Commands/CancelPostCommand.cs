using MassTransit;

namespace Publishy.Api.Modules.Calendar.Commands;

public record CancelPostCommand(string PostId) : Request;