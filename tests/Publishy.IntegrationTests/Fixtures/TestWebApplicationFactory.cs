using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Publishy.IntegrationTests.Fixtures;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly MongoDbFixture _mongoDbFixture;

    public TestWebApplicationFactory(MongoDbFixture mongoDbFixture)
    {
        _mongoDbFixture = mongoDbFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing MongoDB registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMongoClient));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register test MongoDB client
            services.AddSingleton(_mongoDbFixture.MongoClient);
        });
    }
}