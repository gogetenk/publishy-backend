namespace Publishy.Api.Modules.Calendar.Responses;

public record MonthlyCalendarResponse(
    string Month,
    CalendarPostResponse[] Posts
);

public record CalendarPostResponse(
    string PostId,
    string ProjectId,
    string Content,
    string MediaType,
    DateTime ScheduledDate,
    string Status,
    CalendarNetworkSpecs NetworkSpecs
);

public record CalendarNetworkSpecs(
    CalendarTwitterSpecs? Twitter = null,
    CalendarLinkedInSpecs? LinkedIn = null,
    CalendarInstagramSpecs? Instagram = null,
    CalendarBlogSpecs? Blog = null,
    CalendarNewsletterSpecs? Newsletter = null
);

public record CalendarTwitterSpecs(int TweetLength);
public record CalendarLinkedInSpecs(string PostType);
public record CalendarInstagramSpecs(string ImageDimensions);
public record CalendarBlogSpecs(string Category);
public record CalendarNewsletterSpecs(string SubjectLine);