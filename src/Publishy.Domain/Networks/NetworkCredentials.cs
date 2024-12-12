using Ardalis.Result;
using Publishy.Domain.Common.Validation;

namespace Publishy.Domain.Networks;

public record NetworkCredentials
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }

    private NetworkCredentials() { } // For EF Core

    private NetworkCredentials(string clientId, string clientSecret, string accessToken, string refreshToken)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public static Result<NetworkCredentials> Create(string clientId, string clientSecret, string accessToken, string refreshToken)
    {
        var clientIdValidation = DomainValidator.ValidateString(clientId, nameof(clientId));
        if (!clientIdValidation.IsSuccess)
            return Result.Error(clientIdValidation.Errors);

        var clientSecretValidation = DomainValidator.ValidateString(clientSecret, nameof(clientSecret));
        if (!clientSecretValidation.IsSuccess)
            return Result.Error(clientSecretValidation.Errors);

        var accessTokenValidation = DomainValidator.ValidateString(accessToken, nameof(accessToken));
        if (!accessTokenValidation.IsSuccess)
            return Result.Error(accessTokenValidation.Errors);

        var refreshTokenValidation = DomainValidator.ValidateString(refreshToken, nameof(refreshToken));
        if (!refreshTokenValidation.IsSuccess)
            return Result.Error(refreshTokenValidation.Errors);

        return Result.Success(new NetworkCredentials(clientId, clientSecret, accessToken, refreshToken));
    }
}