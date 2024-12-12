namespace Publishy.Domain.Networks;

public interface INetworkRepository
{
    Task<IEnumerable<Network>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Network?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Network> AddAsync(Network network, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}