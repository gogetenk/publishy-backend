using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.CreateCalendar;

public record CreateCalendarCommand(
    string ProjectId,
    string Name,
    string Description,
    List<CalendarEventDto> Events,
    List<string> SharedWith
) : Request<Result<CalendarResponse>>;

public record CalendarEventDto(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Type,
    string Status,
    List<string> Attendees,
    Dictionary<string, string> Metadata
);

public class CreateCalendarCommandHandler : MediatorRequestHandler<CreateCalendarCommand, Result<CalendarResponse>>
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateCalendarCommandHandler(ICalendarRepository calendarRepository, IProjectRepository projectRepository)
    {
        _calendarRepository = calendarRepository;
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<CalendarResponse>> Handle(CreateCalendarCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

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

        var calendarResult = Domain.AggregateRoots.Calendar.Create(
            request.ProjectId,
            request.Name,
            request.Description,
            events,
            request.SharedWith
        );

        if (!calendarResult.IsSuccess)
            return Result.Error(calendarResult.Errors.ToArray());

        var calendar = await _calendarRepository.AddAsync(calendarResult.Value, cancellationToken);
        return Result.Success((CalendarResponse)calendar);
    }
}