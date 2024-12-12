namespace Publishy.Application.Calendar.Handlers;

public class UpdatePostCommandHandler : MediatorRequestHandler<UpdatePostCommand, Result<CalendarPostResponse>>
{
    private readonly ICalendarRepository _calendarRepository;

    public UpdatePostCommandHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    protected override async Task<Result<CalendarPostResponse>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var entry = await _calendarRepository.GetEntryByPostIdAsync(request.PostId, cancellationToken);
        if (entry == null)
            return Result.NotFound($"Calendar entry for post ID {request.PostId} not found");

        var post = Post.Create(
            entry.ProjectId,
            request.Content,
            Enum.Parse<MediaType>(request.MediaType),
            request.ScheduledDate,
            entry.NetworkSpecs
        );

        if (!post.IsSuccess)
            return Result.Error(post.Errors);

        var updateResult = entry.Update(post.Value);
        if (!updateResult.IsSuccess)
            return Result.Error(updateResult.Errors);

        await _calendarRepository.UpdateAsync(entry.Calendar, cancellationToken);
        return Result.Success(CalendarMappers.MapToCalendarPostResponse(entry));
    }
}