using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publishy.Domain.Analytics;
using Publishy.Domain.Calendar;
using Publishy.Domain.Common.Services;
using Publishy.Domain.MarketingPlans;
using Publishy.Domain.Networks;
using Publishy.Domain.Posts;
using Publishy.Domain.Projects;
using Publishy.Infrastructure.Analytics;
using Publishy.Infrastructure.Calendar;
using Publishy.Infrastructure.Common.Services;
using Publishy.Infrastructure.MarketingPlans;
using Publishy.Infrastructure.Networks;
using Publishy.Infrastructure.Persistence.MongoDb;
using Publishy.Infrastructure.Persistence.MongoDb.Settings;
using Publishy.Infrastructure.Posts;
using Publishy.Infrastructure.Projects;

namespace Publishy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDb"));

        // Core Services
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<INetworkRepository, NetworkRepository>();
        services.AddScoped<IMarketingPlanRepository, MarketingPlanRepository>();
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IAnalyticsSnapshotRepository, AnalyticsSnapshotRepository>();

        return services;
    }
}