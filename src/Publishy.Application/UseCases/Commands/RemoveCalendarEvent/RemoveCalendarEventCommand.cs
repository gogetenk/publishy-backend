using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateCalendar;

namespace Publishy.Application.UseCases.Commands.RemoveCalendarEvent;

public record RemoveCalendarEventCommand(
    string CalendarId,
    string EventId
) : Request<Result<CalendarResponse>>;

public class RemoveCalendarEventCommandHandler : MediatorRequestHandler<RemoveCalendarEventCommand, Result<CalendarResponse>>
{
    private readonly ICalendarRepository _calendarRepository;

    public RemoveCalendarEventCommandHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result<CalendarResponse>> Handle(RemoveCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var calendar = await _calendarRepository.GetByIdAsync(request.CalendarId, cancellationToken);
        if (calendar == null)
            return Result.NotFound($"Calendar with ID {request.CalendarId} was not found");

        var removeResult = calendar.RemoveEvent(request.EventId);
        if (!removeResult.IsSuccess)
            return Result.Error(removeResult.Errors.ToArray());

        await _calendarRepository.UpdateAsync(calendar, cancellationToken);
        return Result.Success((CalendarResponse)calendar);
    }
}