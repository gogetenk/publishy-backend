using Microsoft.OpenApi.Models;

namespace Publishy.WebApi;

public class OpenApiOAuthSecurityRequirement : OpenApiSecurityRequirement
{
    public OpenApiOAuthSecurityRequirement()
    {
        this.Add(new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "oauth2"
            },
            Scheme = "oauth2",
            Name = "oauth2",
            In = ParameterLocation.Header
        },
          new List<string>());
    }
}
