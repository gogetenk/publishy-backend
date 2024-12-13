using Publishy.Application.Domain.Entities;

namespace Publishy.Application.Interfaces;

public interface IPublicationAttemptRepository
{
    Task<PublicationAttempt> AddAsync(PublicationAttempt attempt, CancellationToken cancellationToken = default);
    Task<IEnumerable<PublicationAttempt>> GetByPostIdAsync(string postId, CancellationToken cancellationToken = default);
    Task<int> GetFailedAttemptsCountAsync(string postId, TimeSpan window, CancellationToken cancellationToken = default);
}
