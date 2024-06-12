using LearningAspire.Commons;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis(Constants.RedisCache);

var apiService = builder.AddProject<Projects.LearningAspire_ApiService>(Constants.ApiService);

builder.AddProject<Projects.LearningAspire_Web>(Constants.WebFrontend)
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
