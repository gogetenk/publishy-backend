using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateCalendar;

namespace Publishy.Application.UseCases.Commands.AddCalendarEvent;

public record AddCalendarEventCommand(
    string CalendarId,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Type,
    string Status,
    List<string> Attendees,
    Dictionary<string, string> Metadata
) : Request<Result<CalendarResponse>>;

public class AddCalendarEventCommandHandler : MediatorRequestHandler<AddCalendarEventCommand, Result<CalendarResponse>>
{
    private readonly ICalendarRepository _calendarRepository;

    public AddCalendarEventCommandHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result<CalendarResponse>> Handle(AddCalendarEventCommand request, CancellationToken cancellationToken)
    {
        var calendar = await _calendarRepository.GetByIdAsync(request.CalendarId, cancellationToken);
        if (calendar == null)
            return Result.NotFound($"Calendar with ID {request.CalendarId} was not found");

        var calendarEvent = new CalendarEvent(
            Guid.NewGuid().ToString(),
            request.Title,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.Type,
            request.Status,
            request.Attendees,
            request.Metadata
        );

        var addResult = calendar.AddEvent(calendarEvent);
        if (!addResult.IsSuccess)
            return Result.Error(addResult.Errors.ToArray());

        await _calendarRepository.UpdateAsync(calendar, cancellationToken);
        return Result.Success((CalendarResponse)calendar);
    }
}