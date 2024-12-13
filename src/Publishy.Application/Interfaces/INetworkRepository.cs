using Publishy.Application.Domain.AggregateRoots;

namespace Publishy.Application.Interfaces;

public interface INetworkRepository
{
    Task<Network?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Network>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        NetworkStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<Network> AddAsync(Network network, CancellationToken cancellationToken = default);
    Task UpdateAsync(Network network, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(
        string? projectId = null,
        NetworkStatus? status = null,
        CancellationToken cancellationToken = default);
}