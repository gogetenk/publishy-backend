using MassTransit;
using Publishy.Api.Modules.Analytics.Responses;

namespace Publishy.Api.Modules.Analytics.Queries;

public record GetScheduledPostsCountQuery() : Request<ScheduledPostsCountResponse>;