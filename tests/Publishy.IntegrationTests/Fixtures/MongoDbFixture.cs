using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace Publishy.IntegrationTests.Fixtures;

public class MongoDbFixture : IAsyncLifetime
{
    private MongoDbRunner _runner;
    public IMongoClient MongoClient { get; private set; }
    public string ConnectionString { get; private set; }

    public async Task InitializeAsync()
    {
        _runner = MongoDbRunner.Start();
        ConnectionString = _runner.ConnectionString;
        MongoClient = new MongoClient(ConnectionString);
        
        // Initialize database with required collections
        var database = MongoClient.GetDatabase("publishy-db");
        await database.CreateCollectionAsync("Projects");
        await database.CreateCollectionAsync("Posts");
    }

    public Task DisposeAsync()
    {
        _runner?.Dispose();
        return Task.CompletedTask;
    }
}