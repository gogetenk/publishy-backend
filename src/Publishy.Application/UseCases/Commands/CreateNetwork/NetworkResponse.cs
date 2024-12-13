using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.UseCases.Commands.CreateNetwork;

public record NetworkResponse(
    string Id,
    string ProjectId,
    string Name,
    string Description,
    string Status,
    List<NetworkConnection> Connections,
    NetworkMetrics Metrics,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
)
{
    public static explicit operator NetworkResponse(Domain.AggregateRoots.Network network)
    {
        return new NetworkResponse(
            network.Id,
            network.ProjectId,
            network.Name,
            network.Description,
            network.Status.ToString(),
            network.Connections,
            network.Metrics,
            network.CreatedAt,
            network.LastUpdatedAt
        );
    }
}