using Ardalis.Result;

namespace Publishy.Domain.Common.Validation;

public static class DomainValidator
{
    public static Result ValidateNotNull(object? value, string name)
    {
        return value == null
            ? Result.Error($"{name} cannot be null")
            : Result.Success();
    }

    public static Result ValidateString(string? value, string name, int maxLength = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Error($"{name} cannot be empty");

        if (maxLength > 0 && value.Length > maxLength)
            return Result.Error($"{name} cannot be longer than {maxLength} characters");

        return Result.Success();
    }

    public static Result ValidateUri(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Error($"{name} cannot be empty");

        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
            return Result.Error($"{name} must be a valid URL");

        return Result.Success();
    }

    public static Result ValidateCollection<T>(IEnumerable<T>? collection, string name, int maxItems = 0)
    {
        if (collection == null || !collection.Any())
            return Result.Error($"{name} cannot be empty");

        if (maxItems > 0 && collection.Count() > maxItems)
            return Result.Error($"{name} cannot contain more than {maxItems} items");

        return Result.Success();
    }

    public static Result ValidateDate(DateTime date, string name, bool mustBeInFuture = false)
    {
        if (mustBeInFuture && date <= DateTime.UtcNow)
            return Result.Error($"{name} must be in the future");

        return Result.Success();
    }

    public static Result ValidateRange(int value, string name, int min, int max)
    {
        if (value < min || value > max)
            return Result.Error($"{name} must be between {min} and {max}");

        return Result.Success();
    }

    public static Result ValidateMonthFormat(string? month, string name)
    {
        if (string.IsNullOrWhiteSpace(month))
            return Result.Error($"{name} cannot be empty");

        if (!DateTime.TryParseExact(month, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out _))
            return Result.Error($"{name} must be in format YYYY-MM");

        return Result.Success();
    }
}