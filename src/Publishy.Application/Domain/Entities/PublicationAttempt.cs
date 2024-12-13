namespace Publishy.Application.Domain.Entities;

public class PublicationAttempt
{
    public string Id { get; private set; }
    public string PostId { get; private set; }
    public string Platform { get; private set; }
    public DateTime AttemptedAt { get; private set; }
    public bool Succeeded { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    private PublicationAttempt() { } // For MongoDB

    public PublicationAttempt(string postId, string platform, bool succeeded, string? errorMessage = null, int retryCount = 0)
    {
        Id = Guid.NewGuid().ToString();
        PostId = postId;
        Platform = platform;
        AttemptedAt = DateTime.UtcNow;
        Succeeded = succeeded;
        ErrorMessage = errorMessage;
        RetryCount = retryCount;
    }
}
