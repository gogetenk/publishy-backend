using Publishy.Domain.Common.Exceptions;

namespace Publishy.Domain.Calendar.Exceptions;

public class CalendarNotFoundException : DomainException
{
    public CalendarNotFoundException(string calendarId) 
        : base($"Calendar with ID {calendarId} not found.")
    {
        CalendarId = calendarId;
    }

    public string CalendarId { get; }
}