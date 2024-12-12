using Publishy.Api.Modules.Calendar.Responses;
using Publishy.Domain.Calendar;
using Publishy.Domain.Posts;

namespace Publishy.Application.Calendar.Mappers;

public static class CalendarMappers
{
    public static MonthlyCalendarResponse MapToMonthlyCalendarResponse(Calendar calendar) =>
        new(
            calendar.Month,
            calendar.Entries.Select(MapToCalendarPostResponse).ToArray()
        );

    private static CalendarPostResponse MapToCalendarPostResponse(CalendarEntry entry) =>
        new(
            entry.PostId,
            entry.ProjectId,
            entry.Content,
            entry.MediaType.ToString(),
            entry.ScheduledDate,
            entry.Status.ToString(),
            MapToCalendarNetworkSpecs(entry.NetworkSpecs)
        );

    private static CalendarNetworkSpecs MapToCalendarNetworkSpecs(NetworkSpecifications specs) =>
        new(
            specs.Twitter != null ? new CalendarTwitterSpecs(specs.Twitter.TweetLength) : null,
            specs.LinkedIn != null ? new CalendarLinkedInSpecs(specs.LinkedIn.PostType) : null,
            specs.Instagram != null ? new CalendarInstagramSpecs(specs.Instagram.ImageDimensions) : null,
            specs.Blog != null ? new CalendarBlogSpecs(specs.Blog.Category) : null,
            specs.Newsletter != null ? new CalendarNewsletterSpecs(specs.Newsletter.SubjectLine) : null
        );
}