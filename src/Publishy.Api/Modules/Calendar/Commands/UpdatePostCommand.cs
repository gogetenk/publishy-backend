using MassTransit;
using Publishy.Api.Modules.Posts.Responses;

namespace Publishy.Api.Modules.Calendar.Commands;

public record UpdatePostCommand(
    string PostId,
    string Content,
    string MediaType,
    DateTime ScheduledDate,
    string Status
) : Request<PostResponse>;