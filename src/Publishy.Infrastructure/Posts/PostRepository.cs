using MongoDB.Driver;
using Publishy.Domain.Posts;
using Publishy.Infrastructure.Persistence.MongoDb;

namespace Publishy.Infrastructure.Posts;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _posts;

    public PostRepository(MongoDbContext context)
    {
        _posts = context.GetCollection<Post>("Posts");
    }

    public async Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _posts.Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetByProjectIdAsync(
        string projectId,
        string? status = null,
        string? mediaType = null,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Post>.Filter;
        var filter = builder.Eq(p => p.ProjectId, projectId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            filter &= builder.Eq(p => p.Status.ToString(), status);
        }

        if (!string.IsNullOrWhiteSpace(mediaType))
        {
            filter &= builder.Eq(p => p.MediaType.ToString(), mediaType);
        }

        return await _posts.Find(filter)
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
}