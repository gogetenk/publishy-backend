using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.MarketingPlans.Commands;
using Publishy.Api.Modules.Posts.Responses;
using Publishy.Domain.MarketingPlans;
using Publishy.Domain.Posts;
using Publishy.Application.Posts.Mappers;

namespace Publishy.Application.MarketingPlans.Handlers;

public class AddPostToMarketingPlanCommandHandler : MediatorRequestHandler<AddPostToMarketingPlanCommand, Result<PostResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;
    private readonly IPostRepository _postRepository;

    public AddPostToMarketingPlanCommandHandler(
        IMarketingPlanRepository marketingPlanRepository,
        IPostRepository postRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
        _postRepository = postRepository;
    }

    protected override async Task<Result<PostResponse>> Handle(AddPostToMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} not found");

        var postResult = Post.Create(
            request.PlanId,
            request.Content,
            Enum.Parse<MediaType>(request.MediaType),
            request.ScheduledDate,
            new NetworkSpecifications()
        );

        if (!postResult.IsSuccess)
            return Result.Error(postResult.Errors);

        var post = postResult.Value;
        var addResult = plan.AddPost(post);
        if (!addResult.IsSuccess)
            return Result.Error(addResult.Errors);

        await _marketingPlanRepository.UpdateAsync(plan, cancellationToken);
        await _postRepository.AddAsync(post, cancellationToken);

        return Result.Success(PostMappers.MapToPostResponse(post));
    }
}
