using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Publishy.WebApi;

public static class SwaggerExtensions
{
    public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoCard API", Version = "v1.0.0" });

            var securityDefinitionName = "oauth2";
            OpenApiSecurityScheme securityScheme = new OpenApiBearerSecurityScheme();
            OpenApiSecurityRequirement securityRequirement = new OpenApiBearerSecurityRequirement(securityScheme);

            if (securityDefinitionName.ToLower() == "oauth2")
            {
                //securityScheme = new OpenApiOAuthSecurityScheme(builder.Configuration["Auth0:Authority"]);
                securityRequirement = new OpenApiOAuthSecurityRequirement();
            }

            c.AddSecurityDefinition(securityDefinitionName, securityScheme);
            c.AddSecurityRequirement(securityRequirement);

            c.UseInlineDefinitionsForEnums();
            c.EnableAnnotations();

            IncludeXmlComments(c);
        });

        return builder;
    }

    private static void IncludeXmlComments(SwaggerGenOptions c)
    {
        var xmlFiles = new[]
        {
            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
        };

        foreach (var xmlFile in xmlFiles)
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        }
    }

    public static WebApplication UseSwaggerAndUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(settings =>
        {
            settings.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoCard API v1.0");
            settings.OAuthClientId(app.Configuration["Auth0:ClientId"]);
            settings.OAuthClientSecret(app.Configuration["Auth0:ClientSecret"]);
            settings.OAuthAppName($"Auth0 CryptoCard API {app.Environment.EnvironmentName}");
            settings.OAuthUsePkce();
            //settings.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
            //{
            //    { "audience", app.Configuration["Auth0:Audience"] }
            //});
        });

        return app;
    }
}

