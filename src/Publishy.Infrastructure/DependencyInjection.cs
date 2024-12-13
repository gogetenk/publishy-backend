using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Domain.Entities;
using Publishy.Application.Interfaces;
using Publishy.Application.UseCases.Consumers;
using Publishy.Infrastructure.Http;
using Publishy.Infrastructure.Http.Configuration;
using Publishy.Infrastructure.Messaging;
using Publishy.Infrastructure.MongoDb;
using Publishy.Infrastructure.MongoDb.Serializers;
using Publishy.Infrastructure.SocialMedia;
using Publishy.Infrastructure.SocialMedia.Configuration;

namespace Publishy.Infrastructure;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        // Repositories
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<IMarketingPlanRepository, MarketingPlanRepository>();
        builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
        builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        builder.Services.AddScoped<INetworkRepository, NetworkRepository>();
        builder.Services.AddScoped<IPublicationAttemptRepository, PublicationAttemptRepository>();

        // Content Generation Services
        builder.Services.Configure<OpenAIOptions>(
            builder.Configuration.GetSection(OpenAIOptions.SectionName));
        builder.Services.Configure<LumaOptions>(
            builder.Configuration.GetSection(LumaOptions.SectionName));

        builder.Services.AddHttpClient<IContentGenerationService, OpenAIContentService>();
        builder.Services.AddHttpClient<IVideoGenerationService, LumaVideoService>();

        // Social Media Publishers
        builder.Services.Configure<TwitterOptions>(
            builder.Configuration.GetSection(TwitterOptions.SectionName));
        builder.Services.Configure<InstagramOptions>(
            builder.Configuration.GetSection(InstagramOptions.SectionName));

        builder.Services.AddHttpClient<TwitterPublisher>();
        builder.Services.AddHttpClient<InstagramPublisher>();
        builder.Services.AddScoped<ISocialMediaPublisherFactory, SocialMediaPublisherFactory>();

        // Message Bus and Publishing Services
        builder.Services.AddScoped<IPostPublisher, PostPublisher>();
        //builder.Services.AddHostedService<ScheduledPostsProcessor>(); // TODO: Delete?

        builder.Services.AddMassTransit(x =>
        {
            x.AddConsumer<ScheduledPostConsumer>();

            Uri schedulerEndpoint = new Uri("queue:scheduler");
            x.AddMessageScheduler(schedulerEndpoint);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString("rmq"));
                cfg.UseDelayedMessageScheduler(); // Active le scheduler en mémoire

                cfg.ReceiveEndpoint("scheduled-posts", e =>
                {
                    e.ConfigureConsumer<ScheduledPostConsumer>(context);
                });
            });
        });

        // MongoDB
        builder.AddMongoDBClient("publishy-db");
        BsonSerializer.RegisterIdGenerator(typeof(Guid), CombGuidGenerator.Instance);
        BsonClassMap.RegisterClassMap<Project>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
            cm.MapMember(c => c.Status)
              .SetSerializer(new EnumSerializer<ProjectStatus>(BsonType.String));
        });
        BsonClassMap.RegisterClassMap<Post>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
            cm.MapMember(c => c.Status)
              .SetSerializer(new EnumSerializer<PostStatus>(BsonType.String));
        });
        BsonClassMap.RegisterClassMap<MarketingPlan>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
            cm.MapMember(c => c.Status)
              .SetSerializer(new EnumSerializer<MarketingPlanStatus>(BsonType.String));
        });
        BsonClassMap.RegisterClassMap<PublicationAttempt>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
        });
        BsonClassMap.RegisterClassMap<Network>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
        });
        BsonClassMap.RegisterClassMap<Calendar>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
        });
        BsonClassMap.RegisterClassMap<Analytics>(cm =>
        {
            cm.AutoMap();
            cm.MapIdField(p => p.Id);
        });


        return builder;
    }
}