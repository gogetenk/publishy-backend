using MassTransit;
using Publishy.Api.Modules.Calendar.Responses;

namespace Publishy.Api.Modules.Calendar.Queries;

public record GetMonthlyCalendarQuery(string? Month) : Request<MonthlyCalendarResponse>;