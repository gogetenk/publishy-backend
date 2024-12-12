var builder = DistributedApplication.CreateBuilder(args);


var db = builder.AddMongoDB("mongodb");
db.AddDatabase("publishy-db");

var redis = builder.AddRedis("redis");

//var serviceBus = builder.ExecutionContext.IsPublishMode
//    ? builder.AddAzureServiceBus("servicebus")
//    : builder.AddConnectionString("servicebus");

builder.AddProject<Projects.Publishy_WebApi>("publishy-webapi")
    .WithReference(db)
    //.WithReference(serviceBus)
    .WithReference(redis);

builder.Build().Run();
