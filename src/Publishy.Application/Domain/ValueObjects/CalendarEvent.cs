namespace Publishy.Application.Domain.ValueObjects;

public record CalendarEvent(
    string Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Type,
    string Status,
    List<string> Attendees,
    Dictionary<string, string> Metadata
);