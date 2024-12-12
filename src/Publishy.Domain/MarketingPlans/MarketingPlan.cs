using Ardalis.Result;
using Publishy.Domain.MarketingPlans.Timeline;
using Publishy.Domain.Posts;

namespace Publishy.Domain.MarketingPlans;

public class MarketingPlan
{
    public string Id { get; private set; }
    public string Month { get; private set; }
    public string[] Objectives { get; private set; }
    public List<Post> Posts { get; private set; }
    public List<TimelineEntry> Timeline { get; private set; }
    public MarketingPlanStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private MarketingPlan() { } // For EF Core

    private MarketingPlan(string month, string[] objectives)
    {
        Id = Guid.NewGuid().ToString();
        Month = month;
        Objectives = objectives;
        Posts = new List<Post>();
        Timeline = new List<TimelineEntry>();
        Status = MarketingPlanStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<MarketingPlan> Create(string month, string[] objectives)
    {
        if (string.IsNullOrWhiteSpace(month))
            return Result.Error("Month cannot be empty");

        if (!IsValidMonthFormat(month))
            return Result.Error("Month must be in format YYYY-MM");

        if (objectives == null || !objectives.Any())
            return Result.Error("At least one objective is required");

        return Result.Success(new MarketingPlan(month, objectives));
    }

    public Result AddPost(Post post)
    {
        if (Status == MarketingPlanStatus.Finalized)
            return Result.Error("Cannot add posts to a finalized marketing plan");

        if (post == null)
            return Result.Error("Post cannot be null");

        if (!IsPostInMonth(post.ScheduledDate))
            return Result.Error("Post scheduled date must be within the marketing plan month");

        Posts.Add(post);
        return Result.Success();
    }

    public Result AddToTimeline(TimelineEntry entry)
    {
        if (Status == MarketingPlanStatus.Finalized)
            return Result.Error("Cannot modify timeline of a finalized marketing plan");

        if (entry == null)
            return Result.Error("Timeline entry cannot be null");

        if (!IsEntryInMonth(entry.ScheduledTime))
            return Result.Error("Timeline entry scheduled time must be within the marketing plan month");

        Timeline.Add(entry);
        return Result.Success();
    }

    public Result Finalize()
    {
        if (Status == MarketingPlanStatus.Finalized)
            return Result.Error("Marketing plan is already finalized");

        if (!Posts.Any())
            return Result.Error("Cannot finalize a marketing plan without posts");

        if (!Timeline.Any())
            return Result.Error("Cannot finalize a marketing plan without timeline entries");

        Status = MarketingPlanStatus.Finalized;
        return Result.Success();
    }

    private bool IsPostInMonth(DateTime postDate)
    {
        var planDate = DateTime.ParseExact(Month, "yyyy-MM", null);
        return postDate.Year == planDate.Year && postDate.Month == planDate.Month;
    }

    private bool IsEntryInMonth(DateTime entryDate)
    {
        var planDate = DateTime.ParseExact(Month, "yyyy-MM", null);
        return entryDate.Year == planDate.Year && entryDate.Month == planDate.Month;
    }

    private static bool IsValidMonthFormat(string month)
    {
        return DateTime.TryParseExact(month, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out _);
    }
}