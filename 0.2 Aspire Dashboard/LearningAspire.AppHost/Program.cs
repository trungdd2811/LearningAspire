using LearningAspire.AppHost;
using LearningAspire.Commons;

var builder = DistributedApplication.CreateBuilder(args);

#region add components to aspire orchestrator

var insights = builder.ExecutionContext.IsPublishMode
	? builder.AddAzureApplicationInsights(Constants.MyAspireApp)
	: builder.AddConnectionString(Constants.MyAspireApp, "APPLICATIONINSIGHTS_CONNECTION_STRING");
var cache = builder.AddRedis(Constants.RedisCache);

#region add SQL Server

var sqlPassword = builder.AddParameter("sql-password", secret: true);
var sqlServer = builder.AddSqlServer(Constants.EmployeesSQLServer, password: sqlPassword, 1443);

if (builder.ExecutionContext.IsPublishMode)
{
	cache.PublishAsAzureRedis();
	sqlServer.PublishAsAzureSqlDatabase();
}
else
{
	sqlServer.WithDataVolume(Constants.EmployeesSQLServer);
}
var employeesDB = sqlServer.AddDatabase(Constants.EmployeesDB);

#endregion add SQL Server

#region add PostgreSQL

//var postgrePassword = builder.AddParameter("postgresql-password", secret: true);

//var employeesPostgreDB = builder.AddPostgres("pg", password: postgrePassword)
//                        .WithInitBindMount("VolumeMount\\AppHost-postgre-data")
//                        .WithPgAdmin()
//                        .AddDatabase(Constants.EmployeesPostgreDB);

#endregion add PostgreSQL

#endregion add components to aspire orchestrator

#region add components - project resources to aspire orchestrator

var apiService = builder.AddProject<Projects.LearningAspire_ApiService>(Constants.ApiService)
	.WithReference(cache)
	.WithReplicas(1);

var employeesService = builder.AddProject<Projects.Employees_API>(Constants.EmployeesService)
	.WithExternalHttpEndpoints()
	.WithReference(employeesDB)
	.WithReference(cache)
	.WithReplicas(1);

var webFrontEnd = builder.AddProject<Projects.LearningAspire_Web>(Constants.WebFrontend)
	.WithExternalHttpEndpoints()
	.WithReference(cache)
	.WithReference(apiService)
	.WithReference(employeesService)
	.WithReplicas(1);

builder.AddHealthChecksUI("healthchecksui")
	.WithReference(apiService)
	.WithReference(employeesService)
	.WithReference(webFrontEnd)
	// This will make the HealthChecksUI dashboard available from external networks when deployed.
	// In a production environment, you should consider adding authentication to the ingress layer
	// to restrict access to the dashboard.
	.WithExternalHttpEndpoints();

if (builder.ExecutionContext.IsPublishMode)
{
	webFrontEnd.WithReference(insights);
	apiService.WithReference(insights);
	employeesService.WithReference(insights);
}

#endregion add components - project resources to aspire orchestrator

builder.Build().Run();