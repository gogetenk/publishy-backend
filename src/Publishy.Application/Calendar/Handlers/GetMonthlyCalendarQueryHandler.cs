namespace Publishy.Application.Calendar.Handlers;

public class GetMonthlyCalendarQueryHandler : MediatorRequestHandler<GetMonthlyCalendarQuery, Result<MonthlyCalendarResponse>>
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetMonthlyCalendarQueryHandler(
        ICalendarRepository calendarRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _calendarRepository = calendarRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override async Task<Result<MonthlyCalendarResponse>> Handle(GetMonthlyCalendarQuery request, CancellationToken cancellationToken)
    {
        var month = request.Month ?? _dateTimeProvider.UtcNow.ToString("yyyy-MM");
        var calendar = await _calendarRepository.GetByMonthAsync(month, cancellationToken);

        if (calendar == null)
        {
            var calendarResult = Calendar.Create(month, _dateTimeProvider);
            if (!calendarResult.IsSuccess)
                return Result.Error(calendarResult.Errors);

            calendar = calendarResult.Value;
            await _calendarRepository.AddAsync(calendar, cancellationToken);
        }

        return Result.Success(CalendarMappers.MapToMonthlyCalendarResponse(calendar));
    }
}