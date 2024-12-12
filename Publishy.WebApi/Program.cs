using Publishy.Api.Endpoints;
using Publishy.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.AddInfrastructure();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapProjectEndpoints();
app.MapOpenApi();
app.UseHttpsRedirection();

app.Run();