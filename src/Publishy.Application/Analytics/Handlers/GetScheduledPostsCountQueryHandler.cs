using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.Analytics.Queries;
using Publishy.Api.Modules.Analytics.Responses;
using Publishy.Domain.Analytics;

namespace Publishy.Application.Analytics.Handlers;

public class GetScheduledPostsCountQueryHandler : MediatorRequestHandler<GetScheduledPostsCountQuery, Result<ScheduledPostsCountResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetScheduledPostsCountQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<ScheduledPostsCountResponse>> Handle(GetScheduledPostsCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _analyticsRepository.GetScheduledPostsCountAsync(cancellationToken);
        return Result.Success(new ScheduledPostsCountResponse(count));
    }
}