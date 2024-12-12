using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Publishy.Application.Interfaces;
using Publishy.Infrastructure.MongoDb;
using MongoDB.Bson.Serialization.IdGenerators;
using Microsoft.Extensions.Hosting;

namespace Publishy.Infrastructure;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.AddMongoDBClient("mongodb");

        BsonSerializer.RegisterIdGenerator(typeof(Guid), CombGuidGenerator.Instance);

        return builder;
    }
}
