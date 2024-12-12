namespace Publishy.Api.Modules.Networks.Models;

public record NetworkCredentials(
    string ClientId,
    string ClientSecret,
    string AccessToken,
    string RefreshToken
);