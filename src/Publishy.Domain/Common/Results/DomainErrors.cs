using Ardalis.Result;

namespace Publishy.Domain.Common.Results;

public static class DomainErrors
{
    public static class Project
    {
        public static Result NotFound(string projectId) =>
            Result.NotFound($"Project with ID {projectId} not found");

        public static Result InvalidStatus(string currentStatus, string requestedStatus) =>
            Result.Error($"Cannot transition from {currentStatus} to {requestedStatus}");

        public static Result AlreadyExists(string projectName) =>
            Result.Error($"A project with name '{projectName}' already exists");
    }

    public static class Post
    {
        public static Result NotFound(string postId) =>
            Result.NotFound($"Post with ID {postId} not found");

        public static Result InvalidScheduledDate() =>
            Result.Error("Scheduled date must be in the future");

        public static Result AlreadyPublished() =>
            Result.Error("Post has already been published");

        public static Result AlreadyCanceled() =>
            Result.Error("Post has already been canceled");
    }

    public static class MarketingPlan
    {
        public static Result NotFound(string planId) =>
            Result.NotFound($"Marketing plan with ID {planId} not found");

        public static Result AlreadyFinalized() =>
            Result.Error("Marketing plan has already been finalized");

        public static Result InvalidMonth() =>
            Result.Error("Month must be in format YYYY-MM");
    }

    public static class Network
    {
        public static Result NotFound(string networkId) =>
            Result.NotFound($"Network with ID {networkId} not found");

        public static Result AlreadyConnected(string platform) =>
            Result.Error($"Platform {platform} is already connected");

        public static Result AlreadyDisconnected() =>
            Result.Error("Network is already disconnected");
    }

    public static class Calendar
    {
        public static Result NotFound(string calendarId) =>
            Result.NotFound($"Calendar with ID {calendarId} not found");

        public static Result EntryNotFound(string entryId) =>
            Result.NotFound($"Calendar entry with ID {entryId} not found");

        public static Result InvalidMonth() =>
            Result.Error("Month must be in format YYYY-MM");
    }

    public static class Analytics
    {
        public static Result InvalidPercentages() =>
            Result.Error("Percentages must sum up to 100%");

        public static Result NegativeCount() =>
            Result.Error("Count cannot be negative");

        public static Result SnapshotNotFound(string snapshotId) =>
            Result.NotFound($"Analytics snapshot with ID {snapshotId} not found");
    }
}