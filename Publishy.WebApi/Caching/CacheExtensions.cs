namespace Publishy.Api.Caching;

public static class CacheExtensions
{
    public static IServiceCollection AddOutputCacheWithPolicies(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            // Politique par défaut pour les requêtes GET avec une durée de 30 secondes
            options.AddBasePolicy(builder =>
            {
                builder.Expire(TimeSpan.FromSeconds(30))
                       .SetVaryByQuery("page", "pageSize")
                       .Tag("default");
            });

            // Politique pour les projets avec une durée de 1 minute
            options.AddPolicy("Projects", builder =>
            {
                builder.Expire(TimeSpan.FromMinutes(1))
                       .SetVaryByQuery("page", "pageSize", "status", "createdAfter", "createdBefore")
                       .Tag("projects");
            });

            // Politique pour les posts avec une durée de 30 secondes
            options.AddPolicy("Posts", builder =>
            {
                builder.Expire(TimeSpan.FromSeconds(30))
                       .SetVaryByQuery("page", "pageSize", "projectId", "status", "platform", "createdAfter", "createdBefore")
                       .Tag("posts");
            });

            // Politique pour les plans marketing avec une durée de 1 minute
            options.AddPolicy("MarketingPlans", builder =>
            {
                builder.Expire(TimeSpan.FromMinutes(1))
                       .SetVaryByQuery("page", "pageSize", "projectId", "status", "startDateAfter", "startDateBefore")
                       .Tag("marketing-plans");
            });

            // Politique pour les calendriers avec une durée de 1 minute
            options.AddPolicy("Calendars", builder =>
            {
                builder.Expire(TimeSpan.FromMinutes(1))
                       .SetVaryByQuery("page", "pageSize", "projectId", "status")
                       .Tag("calendars");
            });

            // Politique pour les analytics avec une durée de 5 minutes
            options.AddPolicy("Analytics", builder =>
            {
                builder.Expire(TimeSpan.FromMinutes(5))
                       .SetVaryByQuery("page", "pageSize", "projectId", "source", "startDate", "endDate")
                       .Tag("analytics");
            });

            // Politique pour les réseaux avec une durée de 1 minute
            options.AddPolicy("Networks", builder =>
            {
                builder.Expire(TimeSpan.FromMinutes(1))
                       .SetVaryByQuery("page", "pageSize", "projectId", "status")
                       .Tag("networks");
            });
        });

        return services;
    }
}
