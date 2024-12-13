using MassTransit;
using Publishy.Api.Caching;
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
    builder.Services.AddOutputCacheWithPolicies();

    builder.AddInfrastructure();

    var app = builder.Build();

    app.UseSwaggerAndUI();
    app.MapOpenApi();
    app.UseHttpsRedirection();
    app.UseOutputCacheWithInvalidation();
    app.MapDefaultEndpoints();
    app.MapProjectEndpoints();
    app.MapPostEndpoints();
    app.MapMarketingPlanEndpoints();
    app.MapCalendarEndpoints();
    app.MapAnalyticsEndpoints();
    app.MapNetworkEndpoints();
    app.MapDashboardEndpoints();

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
}