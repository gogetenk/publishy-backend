using Ardalis.Result;
using Publishy.Domain.Common.Validation;

namespace Publishy.Domain.Networks;

public record NetworkConnectionSettings
{
    public string CallbackUrl { get; init; }
    public string[] RequiredScopes { get; init; }
    public Dictionary<string, string> AdditionalParameters { get; init; }

    private NetworkConnectionSettings() { } // For EF Core

    private NetworkConnectionSettings(string callbackUrl, string[] requiredScopes, Dictionary<string, string> additionalParameters)
    {
        CallbackUrl = callbackUrl;
        RequiredScopes = requiredScopes;
        AdditionalParameters = additionalParameters;
    }

    public static Result<NetworkConnectionSettings> Create(string callbackUrl, string[] requiredScopes, Dictionary<string, string>? additionalParameters = null)
    {
        var callbackUrlValidation = DomainValidator.ValidateUri(callbackUrl, nameof(callbackUrl));
        if (!callbackUrlValidation.IsSuccess)
            return Result.Error(callbackUrlValidation.Errors);

        var scopesValidation = DomainValidator.ValidateCollection(requiredScopes, nameof(requiredScopes));
        if (!scopesValidation.IsSuccess)
            return Result.Error(scopesValidation.Errors);

        return Result.Success(new NetworkConnectionSettings(
            callbackUrl,
            requiredScopes,
            additionalParameters ?? new Dictionary<string, string>()
        ));
    }
}