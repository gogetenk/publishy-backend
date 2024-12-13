using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateCalendar;

namespace Publishy.Application.UseCases.Queries.GetCalendarById;

public record GetCalendarByIdQuery(string CalendarId) : Request<Result<CalendarResponse>>;

public class GetCalendarByIdQueryHandler : MediatorRequestHandler<GetCalendarByIdQuery, Result<CalendarResponse>>
{
    private readonly ICalendarRepository _calendarRepository;

    public GetCalendarByIdQueryHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result<CalendarResponse>> Handle(GetCalendarByIdQuery request, CancellationToken cancellationToken)
    {
        var calendar = await _calendarRepository.GetByIdAsync(request.CalendarId, cancellationToken);
        if (calendar == null)
            return Result.NotFound($"Calendar with ID {request.CalendarId} was not found");

        return Result.Success((CalendarResponse)calendar);
    }
}