using MassTransit;

namespace Publishy.Api.Modules.Networks.Commands;

public record DeleteNetworkCommand(string NetworkId) : Request;