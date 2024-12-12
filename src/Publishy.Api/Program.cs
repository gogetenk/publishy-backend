using MassTransit;
using Publishy.Api.Modules.Analytics;
using Publishy.Api.Modules.Calendar;
using Publishy.Api.Modules.MarketingPlans;
using Publishy.Api.Modules.Networks;
using Publishy.Api.Modules.Posts;
using Publishy.Api.Modules.Projects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MassTransit.Mediator
builder.Services.AddMediator(cfg => {
    // Register consumers here
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register modules
app.MapProjectEndpoints();
app.MapPostEndpoints();
app.MapNetworkEndpoints();
app.MapMarketingPlanEndpoints();
app.MapCalendarEndpoints();
app.MapAnalyticsEndpoints();

app.Run();