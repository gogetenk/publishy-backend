using Ardalis.Result;
using Publishy.Domain.Common.Validation;

namespace Publishy.Domain.Networks;

public record NetworkConfiguration
{
    public string Platform { get; init; }
    public int PostsPerDay { get; init; }
    public string[] AllowedMediaTypes { get; init; }
    public int MaxCharactersPerPost { get; init; }

    private NetworkConfiguration() { } // For EF Core

    private NetworkConfiguration(string platform, int postsPerDay, string[] allowedMediaTypes, int maxCharactersPerPost)
    {
        Platform = platform;
        PostsPerDay = postsPerDay;
        AllowedMediaTypes = allowedMediaTypes;
        MaxCharactersPerPost = maxCharactersPerPost;
    }

    public static Result<NetworkConfiguration> Create(string platform, int postsPerDay, string[] allowedMediaTypes, int maxCharactersPerPost)
    {
        var platformValidation = DomainValidator.ValidateString(platform, nameof(platform));
        if (!platformValidation.IsSuccess)
            return Result.Error(platformValidation.Errors);

        var postsPerDayValidation = DomainValidator.ValidateRange(postsPerDay, nameof(postsPerDay), 1, 100);
        if (!postsPerDayValidation.IsSuccess)
            return Result.Error(postsPerDayValidation.Errors);

        var mediaTypesValidation = DomainValidator.ValidateCollection(allowedMediaTypes, nameof(allowedMediaTypes));
        if (!mediaTypesValidation.IsSuccess)
            return Result.Error(mediaTypesValidation.Errors);

        var maxCharactersValidation = DomainValidator.ValidateRange(maxCharactersPerPost, nameof(maxCharactersPerPost), 1, 5000);
        if (!maxCharactersValidation.IsSuccess)
            return Result.Error(maxCharactersValidation.Errors);

        return Result.Success(new NetworkConfiguration(platform, postsPerDay, allowedMediaTypes, maxCharactersPerPost));
    }
}