using Ardalis.Result;
using Publishy.Application.Domain.ValueObjects;

namespace Publishy.Application.Domain.AggregateRoots;

public class Network
{
    public string Id { get; private set; }
    public string ProjectId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public NetworkStatus Status { get; private set; }
    public List<NetworkConnection> Connections { get; private set; }
    public NetworkMetrics Metrics { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    private Network() { } // For MongoDB

    private Network(
        string projectId,
        string name,
        string description,
        List<NetworkConnection> connections)
    {
        Id = Guid.NewGuid().ToString();
        ProjectId = projectId;
        Name = name;
        Description = description;
        Status = NetworkStatus.Active;
        Connections = connections;
        Metrics = CalculateMetrics(connections);
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public static Result<Network> Create(
        string projectId,
        string name,
        string description,
        List<NetworkConnection> connections)
    {
        if (string.IsNullOrWhiteSpace(projectId))
            return Result.Error("Project ID cannot be empty");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Error("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Error("Description cannot be empty");

        if (connections == null)
            connections = new List<NetworkConnection>();

        return Result.Success(new Network(projectId, name, description, connections));
    }

    public Result AddConnection(NetworkConnection connection)
    {
        if (Status != NetworkStatus.Active)
            return Result.Error("Cannot modify an inactive network");

        if (Connections.Any(c => 
            c.SourceId == connection.SourceId && 
            c.TargetId == connection.TargetId &&
            c.Type == connection.Type))
        {
            return Result.Error("Connection already exists");
        }

        Connections.Add(connection);
        Metrics = CalculateMetrics(Connections);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveConnection(string sourceId, string targetId, string type)
    {
        if (Status != NetworkStatus.Active)
            return Result.Error("Cannot modify an inactive network");

        var connection = Connections.FirstOrDefault(c => 
            c.SourceId == sourceId && 
            c.TargetId == targetId && 
            c.Type == type);

        if (connection == null)
            return Result.Error("Connection not found");

        Connections.Remove(connection);
        Metrics = CalculateMetrics(Connections);
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateStatus(bool isActive)
    {
        var newStatus = isActive ? NetworkStatus.Active : NetworkStatus.Inactive;
        
        if (Status == newStatus)
            return Result.Error("Network is already in the requested state");

        Status = newStatus;
        LastUpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    private NetworkMetrics CalculateMetrics(List<NetworkConnection> connections)
    {
        var nodes = connections
            .SelectMany(c => new[] { c.SourceId, c.TargetId })
            .Distinct()
            .Count();

        var totalConnections = connections.Count;
        var avgStrength = connections.Any() 
            ? connections.Average(c => c.Strength) 
            : 0;

        var maxPossibleConnections = nodes * (nodes - 1) / 2.0m;
        var density = maxPossibleConnections > 0 
            ? totalConnections / maxPossibleConnections 
            : 0;

        return new NetworkMetrics(
            nodes,
            totalConnections,
            avgStrength,
            density,
            new Dictionary<string, decimal>()
        );
    }
}

public enum NetworkStatus
{
    Active,
    Inactive
}