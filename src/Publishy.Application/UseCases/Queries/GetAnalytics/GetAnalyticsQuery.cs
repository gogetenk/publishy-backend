using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Common.Responses;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateAnalytics;

namespace Publishy.Application.UseCases.Queries.GetAnalytics;

public record GetAnalyticsQuery(
    int Page,
    int PageSize,
    string? ProjectId,
    string? Source,
    DateTime? StartDate,
    DateTime? EndDate
) : Request<Result<GetAnalyticsResponse>>;

public class GetAnalyticsQueryHandler : MediatorRequestHandler<GetAnalyticsQuery, Result<GetAnalyticsResponse>>
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public GetAnalyticsQueryHandler(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    protected override async Task<Result<GetAnalyticsResponse>> Handle(GetAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var analytics = await _analyticsRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.ProjectId,
            request.Source,
            request.StartDate,
            request.EndDate,
            cancellationToken
        );

        var totalItems = await _analyticsRepository.GetTotalCountAsync(
            request.ProjectId,
            request.Source,
            request.StartDate,
            request.EndDate,
            cancellationToken
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var analyticsResponses = analytics.Select(a => (AnalyticsResponse)a).ToArray();

        var response = new GetAnalyticsResponse(
            Data: analyticsResponses,
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