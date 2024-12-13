using Publishy.Application.Common.Responses;
using Publishy.Application.UseCases.Commands.CreateAnalytics;

namespace Publishy.Application.UseCases.Queries.GetAnalytics;

public record GetAnalyticsResponse(
    AnalyticsResponse[] Data,
    PaginationResponse Pagination
);