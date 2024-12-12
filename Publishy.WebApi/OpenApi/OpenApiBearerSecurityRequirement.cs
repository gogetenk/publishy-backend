using Microsoft.OpenApi.Models;

namespace Publishy.WebApi;

public class OpenApiBearerSecurityRequirement : OpenApiSecurityRequirement
{
    public OpenApiBearerSecurityRequirement(OpenApiSecurityScheme securityScheme)
    {
        this.Add(securityScheme, new[] { "Bearer" });
    }
}
