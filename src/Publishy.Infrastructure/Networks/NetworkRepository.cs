using MongoDB.Driver;
using Publishy.Domain.Networks;
using Publishy.Infrastructure.Persistence.MongoDb;

namespace Publishy.Infrastructure.Networks;

public class NetworkRepository : INetworkRepository
{
    private readonly IMongoCollection<Network> _networks;

    public NetworkRepository(MongoDbContext context)
    {
        _networks = context.GetCollection<Network>("Networks");
    }

    public async Task<IEnumerable<Network>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _networks.Find(_ => true)
            .ToListAsync(cancellationToken);
    }

    public async Task<Network?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _networks.Find(n => n.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Network> AddAsync(Network network, CancellationToken cancellationToken = default)
    {
        await _networks.InsertOneAsync(network, cancellationToken: cancellationToken);
        return network;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _networks.DeleteOneAsync(n => n.Id == id, cancellationToken);
    }
}