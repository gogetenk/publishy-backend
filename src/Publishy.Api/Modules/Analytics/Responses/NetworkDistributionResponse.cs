namespace Publishy.Api.Modules.Analytics.Responses;

public record NetworkDistributionResponse(
    string Network,
    NetworkDistributionPercentages Percentages
);

public record NetworkDistributionPercentages(
    float Text,
    float Image,
    float Video
);