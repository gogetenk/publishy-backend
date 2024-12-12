namespace Publishy.Application.Calendar.Handlers;

public class CancelPostCommandHandler : MediatorRequestHandler<CancelPostCommand, Result>
{
    private readonly ICalendarRepository _calendarRepository;

    public CancelPostCommandHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result> Handle(CancelPostCommand request, CancellationToken cancellationToken)
    {
        var entry = await _calendarRepository.GetEntryByPostIdAsync(request.PostId, cancellationToken);
        if (entry == null)
            return Result.NotFound($"Calendar entry for post ID {request.PostId} not found");

        var deleteResult = entry.Calendar.RemoveEntry(entry.Id);
        if (!deleteResult.IsSuccess)
            return Result.Error(deleteResult.Errors);

        await _calendarRepository.DeleteEntryAsync(entry.Id, cancellationToken);
        return Result.Success();
    }
}