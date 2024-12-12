using Publishy.Domain.Common.Exceptions;

namespace Publishy.Domain.Calendar.Exceptions;

public class CalendarEntryNotFoundException : DomainException
{
    public CalendarEntryNotFoundException(string entryId) 
        : base($"Calendar entry with ID {entryId} not found.")
    {
        EntryId = entryId;
    }

    public string EntryId { get; }
}