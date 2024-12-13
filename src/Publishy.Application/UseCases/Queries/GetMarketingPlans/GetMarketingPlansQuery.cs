using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Common.Responses;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateMarketingPlan;

namespace Publishy.Application.UseCases.Queries.GetMarketingPlans;

public record GetMarketingPlansQuery(
    int Page,
    int PageSize,
    string? ProjectId,
    string? Status,
    DateTime? StartDateAfter,
    DateTime? StartDateBefore
) : Request<Result<GetMarketingPlansResponse>>;

public class GetMarketingPlansQueryHandler : MediatorRequestHandler<GetMarketingPlansQuery, Result<GetMarketingPlansResponse>>
{
    private readonly IMarketingPlanRepository _marketingPlanRepository;

    public GetMarketingPlansQueryHandler(IMarketingPlanRepository marketingPlanRepository)
    {
        _marketingPlanRepository = marketingPlanRepository;
    }

    protected override async Task<Result<GetMarketingPlansResponse>> Handle(GetMarketingPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _marketingPlanRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.ProjectId,
            request.Status,
            request.StartDateAfter,
            request.StartDateBefore,
            cancellationToken
        );

        var totalItems = await _marketingPlanRepository.GetTotalCountAsync(
            request.ProjectId,
            request.Status,
            request.StartDateAfter,
            request.StartDateBefore,
            cancellationToken
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var planResponses = plans.Select(plan => (MarketingPlanResponse)plan).ToArray();

        var response = new GetMarketingPlansResponse(
            Data: planResponses,
            Pagination: new PaginationResponse(
                CurrentPage: request.Page,
                PageSize: request.PageSize,
                TotalPages: totalPages,
                TotalItems: totalItems
            )
        );

        return Result.Success(response);
    }
}