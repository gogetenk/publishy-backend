using Ardalis.Result;
using MassTransit.Mediator;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.Interfaces;

namespace Publishy.Application.UseCases.Commands.CreateNetwork;

public record CreateNetworkCommand(
    string ProjectId,
    string Name,
    string Description,
    List<NetworkConnectionDto> Connections
) : Request<Result<NetworkResponse>>;

public record NetworkConnectionDto(
    string SourceId,
    string TargetId,
    string Type,
    decimal Strength,
    Dictionary<string, string> Metadata
);

public class CreateNetworkCommandHandler : MediatorRequestHandler<CreateNetworkCommand, Result<NetworkResponse>>
{
    private readonly INetworkRepository _networkRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateNetworkCommandHandler(INetworkRepository networkRepository, IProjectRepository projectRepository)
    {
        _networkRepository = networkRepository;
        _projectRepository = projectRepository;
    }

    protected override async Task<Result<NetworkResponse>> Handle(CreateNetworkCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            return Result.NotFound($"Project with ID {request.ProjectId} was not found");

        var connections = request.Connections
            .Select(c => new NetworkConnection(
                c.SourceId,
                c.TargetId,
                c.Type,
                c.Strength,
                c.Metadata
            ))
            .ToList();

        var networkResult = Domain.AggregateRoots.Network.Create(
            request.ProjectId,
            request.Name,
            request.Description,
            connections
        );

        if (!networkResult.IsSuccess)
            return Result.Error(networkResult.Errors.ToArray());

        var network = await _networkRepository.AddAsync(networkResult.Value, cancellationToken);
        return Result.Success((NetworkResponse)network);
    }
}