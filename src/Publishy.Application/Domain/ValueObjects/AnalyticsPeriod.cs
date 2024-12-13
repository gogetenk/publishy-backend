namespace Publishy.Application.Domain.ValueObjects;

public record AnalyticsPeriod(
    DateTime StartDate,
    DateTime EndDate
)
{
    public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;
    public int GetTotalDays() => (EndDate - StartDate).Days + 1;
}