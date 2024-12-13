using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Commands.CreateCalendar;

public record CalendarResponse(
    string Id,
    string ProjectId,
    string Name,
    string Description,
    string Status,
    List<CalendarEvent> Events,
    List<string> SharedWith,
    DateTime CreatedAt,
    DateTime LastModifiedAt
)
{
    public static explicit operator CalendarResponse(Domain.AggregateRoots.Calendar calendar)
    {
        return new CalendarResponse(
            calendar.Id,
            calendar.ProjectId,
            calendar.Name,
            calendar.Description,
            calendar.Status.ToString(),
            calendar.Events,
            calendar.SharedWith,
            calendar.CreatedAt,
            calendar.LastModifiedAt
        );
    }
}