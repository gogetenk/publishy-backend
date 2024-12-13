using Publishy.Application.Domain.AggregateRoots;

namespace Publishy.Application.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        string? status = null,
        string? platform = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default);
    Task<Post> AddAsync(Post post, CancellationToken cancellationToken = default);
    Task UpdateAsync(Post post, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(
        string? projectId = null,
        string? status = null,
        string? platform = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetScheduledPostsAsync(DateTime before, CancellationToken cancellationToken = default);
}