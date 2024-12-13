using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.DeleteMarketingPlan;

public record DeleteMarketingPlanCommand(string PlanId) : Request<Result>;

public class DeleteMarketingPlanCommandHandler : MediatorRequestHandler<DeleteMarketingPlanCommand, Result>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public DeleteMarketingPlanCommandHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result> Handle(DeleteMarketingPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _marketingPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null)
            return Result.NotFound($"Marketing plan with ID {request.PlanId} was not found");

        await _marketingPlanRepository.DeleteAsync(request.PlanId, cancellationToken);
        return Result.Success();
    }
}