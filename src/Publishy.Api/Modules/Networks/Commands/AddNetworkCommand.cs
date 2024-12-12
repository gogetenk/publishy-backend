using MassTransit;
using Publishy.Api.Modules.Networks.Models;
using Publishy.Api.Modules.Networks.Responses;

namespace Publishy.Api.Modules.Networks.Commands;

public record AddNetworkCommand(
    string Platform,
    NetworkCredentials Credentials
) : Request<NetworkResponse>;