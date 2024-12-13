using Ardalis.Result;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.Domain.AggregateRoots;

public class MarketingPlan
{
    public string Id { get; private set; }
    public string ProjectId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public MarketingPlanStatus Status { get; private set; }
    public List<MarketingGoal> Goals { get; private set; }
    public List<ContentStrategy> ContentStrategies { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastModifiedAt { get; private set; }

    private MarketingPlan() { } // For MongoDB

    private MarketingPlan(
        string projectId,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        List<MarketingGoal> goals,
        List<ContentStrategy> contentStrategies)
    {
        Id = Guid.NewGuid().ToString();
        ProjectId = projectId;
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Status = MarketingPlanStatus.Draft;
        Goals = goals;
        ContentStrategies = contentStrategies;
        CreatedAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public static Result<MarketingPlan> Create(
        string projectId,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        List<MarketingGoal> goals,
        List<ContentStrategy> contentStrategies)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return Result.Error("Project ID cannot be empty");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Description cannot be empty");

        if (startDate >= endDate)
            return Result.Error("Start date must be before end date");

        if (startDate < DateTime.UtcNow)
            return Result.Error("Start date cannot be in the past");

        if (!goals.Any())
            return Result.Error("At least one goal must be specified");

        if (!contentStrategies.Any())
            return Result.Error("At least one content strategy must be specified");

        return Result.Success(new MarketingPlan(
            projectId,
            name,
            description,
            startDate,
            endDate,
            goals,
            contentStrategies));
    }

    public Result Update(
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        List<MarketingGoal> goals,
        List<ContentStrategy> contentStrategies)
    {
        if (Status == MarketingPlanStatus.Active)
            return Result.Error("Cannot update an active marketing plan");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Description cannot be empty");

        if (startDate >= endDate)
            return Result.Error("Start date must be before end date");

        if (startDate < DateTime.UtcNow)
            return Result.Error("Start date cannot be in the past");

        if (!goals.Any())
            return Result.Error("At least one goal must be specified");

        if (!contentStrategies.Any())
            return Result.Error("At least one content strategy must be specified");

        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Goals = goals;
        ContentStrategies = contentStrategies;
        LastModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == MarketingPlanStatus.Active)
            return Result.Error("Marketing plan is already active");

        if (DateTime.UtcNow < StartDate)
            return Result.Error("Cannot activate a marketing plan before its start date");

        Status = MarketingPlanStatus.Active;
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (Status != MarketingPlanStatus.Active)
            return Result.Error("Marketing plan is not active");

        Status = MarketingPlanStatus.Inactive;
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }
}

public enum MarketingPlanStatus
{
    Draft,
    Active,
    Inactive
}