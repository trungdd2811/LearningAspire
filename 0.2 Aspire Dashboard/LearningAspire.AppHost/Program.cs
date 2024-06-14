using LearningAspire.Commons;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

#region add components to aspire orchestrator 
var insights = builder.AddAzureApplicationInsights(Constants.MyAspireApp);
var cache = builder.AddRedis(Constants.RedisCache);
var sqlServer = builder.AddSqlServer(Constants.SqlServer);
sqlServer.AddDatabase(Constants.SqlDB);

var isDevelopment = builder.Environment.IsDevelopment();
if (!isDevelopment)
{
    cache.PublishAsAzureRedis();
    sqlServer.PublishAsAzureSqlDatabase();
} 
#endregion

#region add projects to aspire orchestrator 
var apiService = builder.AddProject<Projects.LearningAspire_ApiService>(Constants.ApiService)
    .WithReplicas(2);

var employeesService = builder.AddProject<Projects.EmployeesAPI>(Constants.EmployeesService)
    .WithReplicas(2);

var webFrontEnd = builder.AddProject<Projects.LearningAspire_Web>(Constants.WebFrontend)
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(employeesService)
    .WithReplicas(2);
#endregion

if (!isDevelopment)
{;
    webFrontEnd.WithReference(insights);
    apiService.WithReference(insights);
}


builder.Build().Run();
