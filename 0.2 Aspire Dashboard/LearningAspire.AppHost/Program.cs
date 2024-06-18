using LearningAspire.Commons;

var builder = DistributedApplication.CreateBuilder(args);

#region add components to aspire orchestrator 
var insights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights(Constants.MyAspireApp)
    : builder.AddConnectionString(Constants.MyAspireApp, "APPLICATIONINSIGHTS_CONNECTION_STRING"); ;
var cache = builder.AddRedis(Constants.RedisCache);

#region add SQL Server
var sqlPassword = builder.AddParameter("sql-password", secret: true);
var sqlServer = builder.AddSqlServer(Constants.EmployeesSQLServer, password: sqlPassword);


if (builder.ExecutionContext.IsPublishMode)
{
    cache.PublishAsAzureRedis();
    sqlServer.PublishAsAzureSqlDatabase();
}
else
{
    sqlServer.WithDataBindMount("VolumeMount\\AppHost-sql-data");
}
var employeesDB = sqlServer.AddDatabase(Constants.EmployeesDB);
#endregion

#region add PostgreSQL

//var postgrePassword = builder.AddParameter("postgresql-password", secret: true);

//var employeesPostgreDB = builder.AddPostgres("pg", password: postgrePassword)
//                        .WithInitBindMount("VolumeMount\\AppHost-postgre-data")
//                        .WithPgAdmin()
//                        .AddDatabase(Constants.EmployeesPostgreDB);
#endregion

#endregion

#region add projects to aspire orchestrator 
var apiService = builder.AddProject<Projects.LearningAspire_ApiService>(Constants.ApiService)
    .WithReplicas(1);


var employeesService = builder.AddProject<Projects.Employees_API>(Constants.EmployeesService)
    .WithReference(employeesDB)
    .WithReplicas(1);

var webFrontEnd = builder.AddProject<Projects.LearningAspire_Web>(Constants.WebFrontend)
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(employeesService)
    .WithReplicas(1);
#endregion

if (builder.ExecutionContext.IsPublishMode)
{
    webFrontEnd.WithReference(insights);
    apiService.WithReference(insights);
}


builder.Build().Run();
