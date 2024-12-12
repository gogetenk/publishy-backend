using Publishy.Api.Modules.Analytics.Responses;
using Publishy.Domain.Analytics;

namespace Publishy.Application.Analytics.Mappers;

public static class AnalyticsMappers
{
    public static GlobalPerformanceResponse MapToGlobalPerformanceResponse(GlobalPerformance performance) =>
        new(performance.TotalPublishedPosts, performance.TotalProjects);

    public static NetworkDistributionResponse MapToNetworkDistributionResponse(NetworkDistribution distribution) =>
        new(
            distribution.Network,
            new NetworkDistributionPercentages(
                distribution.Percentages.Text,
                distribution.Percentages.Image,
                distribution.Percentages.Video
            )
        );
}