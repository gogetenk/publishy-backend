using Microsoft.OpenApi.Models;

namespace Publishy.WebApi;

public class OpenApiOAuthSecurityScheme : OpenApiSecurityScheme
{
    public OpenApiOAuthSecurityScheme(string domain)
    {
        Type = SecuritySchemeType.OAuth2;
        Flows = new OpenApiOAuthFlows()
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{domain}/authorize"),
                TokenUrl = new Uri($"{domain}/oauth/token")
            }
        };
    }
}
