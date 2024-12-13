using MongoDB.Driver;
using Publishy.Application.Domain.Entities;
using Publishy.Application.Interfaces;

namespace Publishy.Infrastructure.MongoDb;

public class PublicationAttemptRepository : IPublicationAttemptRepository
{
    private readonly IMongoCollection<PublicationAttempt> _attempts;

    public PublicationAttemptRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("publishy-db");
        _attempts = database.GetCollection<PublicationAttempt>("PublicationAttempts");
    }

    public async Task<PublicationAttempt> AddAsync(PublicationAttempt attempt, CancellationToken cancellationToken = default)
    {
        await _attempts.InsertOneAsync(attempt, cancellationToken: cancellationToken);
        return attempt;
    }

    public async Task<IEnumerable<PublicationAttempt>> GetByPostIdAsync(string postId, CancellationToken cancellationToken = default)
    {
        return await _attempts.Find(a => a.PostId == postId)
            .SortByDescending(a => a.AttemptedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetFailedAttemptsCountAsync(string postId, TimeSpan window, CancellationToken cancellationToken = default)
    {
        var cutoffTime = DateTime.UtcNow.Subtract(window);
        
        var filter = Builders<PublicationAttempt>.Filter.And(
            Builders<PublicationAttempt>.Filter.Eq(a => a.PostId, postId),
            Builders<PublicationAttempt>.Filter.Eq(a => a.Succeeded, false),
            Builders<PublicationAttempt>.Filter.Gte(a => a.AttemptedAt, cutoffTime)
        );

        return (int)await _attempts.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}
