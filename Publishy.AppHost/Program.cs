var builder = DistributedApplication.CreateBuilder(args);


var db = builder.AddMongoDB("mongodb")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithMongoExpress()
    .WithDataVolume();

var backenddb = db.AddDatabase("publishy-db");

var rmq = builder.AddRabbitMQ("rmq")
    .WithManagementPlugin();

var redis = builder.AddRedis("redis");

//var serviceBus = builder.ExecutionContext.IsPublishMode
//    ? builder.AddAzureServiceBus("servicebus")
//    : builder.AddConnectionString("servicebus");

builder.AddProject<Projects.Publishy_WebApi>("publishy-webapi")
    .WithReference(backenddb)
    .WithReference(rmq)
    //.WithReference(serviceBus)
    .WithReference(redis);

builder.Build().Run();
