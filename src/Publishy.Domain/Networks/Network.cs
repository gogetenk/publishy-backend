using Ardalis.Result;
using Publishy.Domain.Common.Results;
using Publishy.Domain.Common.Validation;

namespace Publishy.Domain.Networks;

public class Network
{
    public string Id { get; private set; }
    public string Platform { get; private set; }
    public NetworkCredentials Credentials { get; private set; }
    public NetworkStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Network() { } // For EF Core

    private Network(string platform, NetworkCredentials credentials)
    {
        Id = Guid.NewGuid().ToString();
        Platform = platform;
        Credentials = credentials;
        Status = NetworkStatus.Connected;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Network> Create(string platform, NetworkCredentials credentials)
    {
        var platformValidation = DomainValidator.ValidateString(platform, nameof(platform));
        if (!platformValidation.IsSuccess)
            return Result.Error(platformValidation.Errors);

        var credentialsValidation = DomainValidator.ValidateNotNull(credentials, nameof(credentials));
        if (!credentialsValidation.IsSuccess)
            return Result.Error(credentialsValidation.Errors);

        var credentialsResult = NetworkCredentials.Create(
            credentials.ClientId,
            credentials.ClientSecret,
            credentials.AccessToken,
            credentials.RefreshToken
        );

        if (!credentialsResult.IsSuccess)
            return Result.Error(credentialsResult.Errors);

        return Result.Success(new Network(platform, credentialsResult.Value));
    }

    public Result Disconnect()
    {
        if (Status == NetworkStatus.Disconnected)
            return DomainErrors.Network.AlreadyDisconnected();

        Status = NetworkStatus.Disconnected;
        return Result.Success();
    }

    public Result RefreshToken(string newAccessToken, string newRefreshToken)
    {
        if (Status == NetworkStatus.Disconnected)
            return Result.Error("Cannot refresh tokens for a disconnected network");

        var accessTokenValidation = DomainValidator.ValidateString(newAccessToken, nameof(newAccessToken));
        if (!accessTokenValidation.IsSuccess)
            return Result.Error(accessTokenValidation.Errors);

        var refreshTokenValidation = DomainValidator.ValidateString(newRefreshToken, nameof(newRefreshToken));
        if (!refreshTokenValidation.IsSuccess)
            return Result.Error(refreshTokenValidation.Errors);

        var credentialsResult = NetworkCredentials.Create(
            Credentials.ClientId,
            Credentials.ClientSecret,
            newAccessToken,
            newRefreshToken
        );

        if (!credentialsResult.IsSuccess)
            return Result.Error(credentialsResult.Errors);

        Credentials = credentialsResult.Value;
        return Result.Success();
    }
}