using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Commands.CreateCalendar;

namespace Publishy.Application.UseCases.Commands.UpdateCalendar;

public record UpdateCalendarCommand(
    string CalendarId,
    string Name,
    string Description,
    List<CalendarEventDto> Events,
    List<string> SharedWith
) : Request<Result<CalendarResponse>>;

public class UpdateCalendarCommandHandler : MediatorRequestHandler<UpdateCalendarCommand, Result<CalendarResponse>>
{
    private readonly ICalendarRepository _calendarRepository;

    public UpdateCalendarCommandHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result<CalendarResponse>> Handle(UpdateCalendarCommand request, CancellationToken cancellationToken)
    {
        var calendar = await _calendarRepository.GetByIdAsync(request.CalendarId, cancellationToken);
        if (calendar == null)
            return Result.NotFound($"Calendar with ID {request.CalendarId} was not found");

        var events = request.Events
            .Select(e => new CalendarEvent(
                Guid.NewGuid().ToString(),
                e.Title,
                e.Description,
                e.StartDate,
                e.EndDate,
                e.Type,
                e.Status,
                e.Attendees,
                e.Metadata
            ))
            .ToList();

        var updateResult = calendar.Update(
            request.Name,
            request.Description,
            events,
            request.SharedWith
        );

        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors.ToArray());

        await _calendarRepository.UpdateAsync(calendar, cancellationToken);
        return Result.Success((CalendarResponse)calendar);
    }
}