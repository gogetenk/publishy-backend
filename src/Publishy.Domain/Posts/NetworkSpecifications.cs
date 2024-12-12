namespace Publishy.Domain.Posts;

public record NetworkSpecifications
{
    public TwitterSpecifications? Twitter { get; init; }
    public LinkedInSpecifications? LinkedIn { get; init; }
    public InstagramSpecifications? Instagram { get; init; }
    public BlogSpecifications? Blog { get; init; }
    public NewsletterSpecifications? Newsletter { get; init; }
}

public record TwitterSpecifications(int TweetLength);
public record LinkedInSpecifications(string PostType);
public record InstagramSpecifications(string ImageDimensions);
public record BlogSpecifications(string Category);
public record NewsletterSpecifications(string SubjectLine);