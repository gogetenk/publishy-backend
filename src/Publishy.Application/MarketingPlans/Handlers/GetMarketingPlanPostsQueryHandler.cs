using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.MarketingPlans.Queries;
using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.MarketingPlans;
using Publishy.Application.Posts.Mappers;

namespace Publishy.Application.MarketingPlans.Handlers;

public class GetMarketingPlanPostsQueryHandler : MediatorRequestHandler<GetMarketingPlanPostsQuery, Result<PostResponse[]>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public GetMarketingPlanPostsQueryHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<PostResponse[]>> Handle(GetMarketingPlanPostsQuery request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} not found");

        var posts = await _marketingPlanRepository.GetPostsAsync(request.PlanId, cancellationToken);
        return Result.Success(posts.Select(PostMappers.MapToPostResponse).ToArray());
    }
}
