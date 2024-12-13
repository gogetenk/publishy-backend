using Publishy.Application.Common.Responses;
using Publishy.Application.UseCases.Commands.CreateCalendar;

namespace Publishy.Application.UseCases.Queries.GetCalendars;

public record GetCalendarsResponse(
    CalendarResponse[] Data,
    PaginationResponse Pagination
);