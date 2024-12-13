using MongoDB.Driver;
using Publishy.Application.Domain.AggregateRoots;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.MongoDb;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _posts;

    public PostRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("publishy-db");
        _posts = database.GetCollection<Post>("Posts");
    }

    public async Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _posts.Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetAllAsync(
        int page,
        int pageSize,
        string? projectId = null,
        PostStatus? status = null,
        string? platform = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Post>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(p => p.ProjectId, projectId);
        }

        if (!status.HasValue)
        {
            filter &= builder.Eq(p => p.Status, status);
        }

        if (!string.IsNullOrWhiteSpace(platform))
        {
            filter &= builder.Eq(p => p.Platform, platform);
        }

        if (createdAfter.HasValue)
        {
            filter &= builder.Gte(p => p.CreatedAt, createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            filter &= builder.Lte(p => p.CreatedAt, createdBefore.Value);
        }

        return await _posts.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Post> AddAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _posts.InsertOneAsync(post, cancellationToken: cancellationToken);
        return post;
    }

    public async Task UpdateAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _posts.ReplaceOneAsync(
            p => p.Id == post.Id,
            post,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _posts.DeleteOneAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(
        string? projectId = null,
        PostStatus? status = null,
        string? platform = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Post>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            filter &= builder.Eq(p => p.ProjectId, projectId);
        }

        if (status.HasValue)
        {
            filter &= builder.Eq(p => p.Status, status);
        }

        if (!string.IsNullOrWhiteSpace(platform))
        {
            filter &= builder.Eq(p => p.Platform, platform);
        }

        if (createdAfter.HasValue)
        {
            filter &= builder.Gte(p => p.CreatedAt, createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            filter &= builder.Lte(p => p.CreatedAt, createdBefore.Value);
        }

        return (int)await _posts.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetScheduledPostsAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Post>.Filter.And(
            Builders<Post>.Filter.Eq(p => p.Status, PostStatus.Scheduled),
            Builders<Post>.Filter.Lte(p => p.ScheduledFor, before)
        );

        return await _posts.Find(filter).ToListAsync(cancellationToken);
    }
}