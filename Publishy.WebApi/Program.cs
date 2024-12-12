using MassTransit;
using Publishy.Api.Endpoints;
using Publishy.Infrastructure;
using Publishy.WebApi;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddServiceDefaults();

    builder.Services.AddProblemDetails();
    builder.Services.AddOpenApi();
    builder.Services.AddMassTransit();
    builder.ConfigureSwagger();

    builder.AddInfrastructure();

    var app = builder.Build();

    app.UseSwaggerAndUI();
    app.MapOpenApi();
    app.UseHttpsRedirection();
    app.MapDefaultEndpoints();
    app.MapProjectEndpoints();

    app.Run();
}
catch (Exception exc)
{
    throw;
}

public partial class Program
{
    protected Program()
    {
    }
} // Needed for IntegrationTests
