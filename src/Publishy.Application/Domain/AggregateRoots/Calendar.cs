using Ardalis.Result;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.Domain.AggregateRoots;

public class Calendar
{
    public string Id { get; private set; }
    public string ProjectId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CalendarStatus Status { get; private set; }
    public List<CalendarEvent> Events { get; private set; }
    public List<string> SharedWith { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastModifiedAt { get; private set; }

    private Calendar() { } // For MongoDB

    private Calendar(
        string projectId,
        string name,
        string description,
        List<CalendarEvent> events,
        List<string> sharedWith)
    {
        Id = Guid.NewGuid().ToString();
        ProjectId = projectId;
        Name = name;
        Description = description;
        Status = CalendarStatus.Active;
        Events = events;
        SharedWith = sharedWith;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public static Result<Calendar> Create(
        string projectId,
        string name,
        string description,
        List<CalendarEvent> events,
        List<string> sharedWith)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return Result.Error("Project ID cannot be empty");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Calendar name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Calendar description cannot be empty");

        if (events == null)
            events = new List<CalendarEvent>();

        if (sharedWith == null)
            sharedWith = new List<string>();

        return Result.Success(new Calendar(projectId, name, description, events, sharedWith));
    }

    public Result Update(
        string name,
        string description,
        List<CalendarEvent> events,
        List<string> sharedWith)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Calendar name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Calendar description cannot be empty");

        Name = name;
        Description = description;
        Events = events ?? new List<CalendarEvent>();
        SharedWith = sharedWith ?? new List<string>();
        LastModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result AddEvent(CalendarEvent calendarEvent)
    {
        if (calendarEvent == null)
            return Result.Error("Event cannot be null");

        if (Events.Any(e => e.StartDate <= calendarEvent.EndDate && e.EndDate >= calendarEvent.StartDate))
            return Result.Error("Event time slot conflicts with existing events");

        Events.Add(calendarEvent);
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveEvent(string eventId)
    {
        var eventToRemove = Events.FirstOrDefault(e => e.Id == eventId);
        if (eventToRemove == null)
            return Result.Error("Event not found");

        Events.Remove(eventToRemove);
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Share(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return Result.Error("User ID cannot be empty");

        if (SharedWith.Contains(userId))
            return Result.Error("Calendar is already shared with this user");

        SharedWith.Add(userId);
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Unshare(string userId)
    {
        if (!SharedWith.Contains(userId))
            return Result.Error("Calendar is not shared with this user");

        SharedWith.Remove(userId);
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == CalendarStatus.Archived)
            return Result.Error("Calendar is already archived");

        Status = CalendarStatus.Archived;
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Restore()
    {
        if (Status == CalendarStatus.Active)
            return Result.Error("Calendar is already active");

        Status = CalendarStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }
}

public enum CalendarStatus
{
    Active,
    Archived
}