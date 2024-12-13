namespace Publishy.Application.Common.Responses;

public record PaginationResponse(
    int CurrentPage,
    int PageSize,
    int TotalPages,
    int TotalItems
);