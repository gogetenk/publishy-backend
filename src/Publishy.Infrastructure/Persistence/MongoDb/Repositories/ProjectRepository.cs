using MongoDB.Driver;
using Publishy.Domain.Projects;

namespace Publishy.Infrastructure.Persistence.MongoDb.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly IMongoCollection<Project> _projects;

    public ProjectRepository(MongoDbContext context)
    {
        _projects = context.GetCollection<Project>("Projects");
    }

    public async Task<Project?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _projects.Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(
        int page,
        int pageSize,
        string? status = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Project>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(p => p.Status.ToString(), status);
        }

        if (createdAfter.HasValue)
        {
            filter &= builder.Gte(p => p.CreatedAt, createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            filter &= builder.Lte(p => p.CreatedAt, createdBefore.Value);
        }

        return await _projects.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _projects.Find(p => p.Status == ProjectStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _projects.InsertOneAsync(project, cancellationToken: cancellationToken);
        return project;
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _projects.ReplaceOneAsync(
            p => p.Id == project.Id,
            project,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _projects.DeleteOneAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? status = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Project>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(p => p.Status.ToString(), status);
        }

        if (createdAfter.HasValue)
        {
            filter &= builder.Gte(p => p.CreatedAt, createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            filter &= builder.Lte(p => p.CreatedAt, createdBefore.Value);
        }

        return (int)await _projects.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}