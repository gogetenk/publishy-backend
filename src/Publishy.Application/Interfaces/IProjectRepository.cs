using Publishy.Application.Domain.AggregateRoots;

namespace Publishy.Application.Interfaces;


public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize, string? status = null, DateTime? createdAfter = null, DateTime? createdBefore = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(string? status = null, DateTime? createdAfter = null, DateTime? createdBefore = null, CancellationToken cancellationToken = default);
}