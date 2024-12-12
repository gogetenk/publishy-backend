using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Api.Modules.MarketingPlans.Commands;
using Publishy.Api.Modules.MarketingPlans.Responses;
using Publishy.Domain.MarketingPlans;
using Publishy.Domain.MarketingPlans.Timeline;
using Publishy.Domain.Posts;

namespace Publishy.Application.MarketingPlans.Handlers;

public class AddPostToTimelineCommandHandler : MediatorRequestHandler<AddPostToTimelineCommand, Result<TimelineEntryResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public AddPostToTimelineCommandHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<TimelineEntryResponse>> Handle(AddPostToTimelineCommand request, CancellationToken cancellationToken)
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
        var timelineEntryResult = TimelineEntry.Create(
            post.Id,
            $"Post for {request.ScheduledDate:MMM dd, yyyy}",
            post.MediaType,
            post.ScheduledDate
        );

        if (!timelineEntryResult.IsSuccess)
            return Result.Error(timelineEntryResult.Errors);

        var addResult = plan.AddToTimeline(timelineEntryResult.Value);
        if (!addResult.IsSuccess)
            return Result.Error(addResult.Errors);

        await _marketingPlanRepository.UpdateAsync(plan, cancellationToken);

        return Result.Success(new TimelineEntryResponse(
            timelineEntryResult.Value.PostId,
            timelineEntryResult.Value.Title,
            timelineEntryResult.Value.MediaType.ToString(),
            timelineEntryResult.Value.ScheduledTime
        ));
    }
}
