using MassTransit;
using Publishy.Api.Modules.Networks.Responses;

namespace Publishy.Api.Modules.Networks.Queries;

public record GetNetworksQuery() : Request<NetworkResponse[]>;