var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Publishy_WebApi>("publishy-webapi");

var db = builder.AddMongoDB("mongodb");
db.AddDatabase("publishy-db");

var redis = builder.AddRedis("redis");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("servicebus")
    : builder.AddConnectionString("servicebus");

builder.Build().Run();
