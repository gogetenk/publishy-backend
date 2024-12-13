using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Common.Responses;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateCalendar;

namespace Publishy.Application.UseCases.Queries.GetCalendars;

public record GetCalendarsQuery(
    int Page,
    int PageSize,
    string? ProjectId,
    string? Status
) : Request<Result<GetCalendarsResponse>>;

public class GetCalendarsQueryHandler : MediatorRequestHandler<GetCalendarsQuery, Result<GetCalendarsResponse>>
{
    private readonly ICalendarRepository _calendarRepository;

    public GetCalendarsQueryHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result<GetCalendarsResponse>> Handle(GetCalendarsQuery request, CancellationToken cancellationToken)
    {
        var calendars = await _calendarRepository.GetAllAsync(
            request.Page,
            request.PageSize,
            request.ProjectId,
            request.Status,
            cancellationToken
        );

        var totalItems = await _calendarRepository.GetTotalCountAsync(
            request.ProjectId,
            request.Status,
            cancellationToken
        );

        var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var calendarResponses = calendars.Select(calendar => (CalendarResponse)calendar).ToArray();

        var response = new GetCalendarsResponse(
            Data: calendarResponses,
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