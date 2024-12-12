namespace Publishy.Domain.Analytics;

public class GlobalPerformance
{
    public int TotalPublishedPosts { get; private set; }
    public int TotalProjects { get; private set; }

    public GlobalPerformance(int totalPublishedPosts, int totalProjects)
    {
        TotalPublishedPosts = totalPublishedPosts;
        TotalProjects = totalProjects;
    }
}