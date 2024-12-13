using MongoDB.Driver;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.MongoDb;

public class NetworkRepository : INetworkRepository
{
    private readonly IMongoCollection<Network> _networks;

    public NetworkRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("publishy-db");
        _networks = database.GetCollection<Network>("Networks");
    }

    public async Task<Network?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _networks.Find(n => n.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Network>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Network>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(n => n.ProjectId, projectId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(n => n.Status.ToString(), status);
        }

        return await _networks.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Network> AddAsync(Network network, CancellationToken cancellationToken = default)
    {
        await _networks.InsertOneAsync(network, cancellationToken: cancellationToken);
        return network;
    }

    public async Task UpdateAsync(Network network, CancellationToken cancellationToken = default)
    {
        await _networks.ReplaceOneAsync(
            n => n.Id == network.Id,
            network,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _networks.DeleteOneAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? projectId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Network>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(n => n.ProjectId, projectId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(n => n.Status.ToString(), status);
        }

        return (int)await _networks.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}