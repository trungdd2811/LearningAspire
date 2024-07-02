# Lessons learned when using Health Checks - UI

* It is an "open source" dashboard. Please see the details here: https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks
* For Aspire projects, the implementation is different so please view the example project below: 
	* https://github.com/dotnet/aspire-samples/tree/main/samples/HealthChecksUI

## 1 - The SSL connection could not be established
```csharp

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
	app.UseHttpsRedirection(); => move it here
}
//we should apply this policy for production environment only to avoid the error "The SSL connection could not be established"
//app.UseHttpsRedirection();

```
![healthchecks-ssl-issue](./Images/healthchecks-ssl-issue.PNG)

## 2 - Automatically adding health-checks for the dependent components
When integrating this health-checks dashboard with aspire projects, it added necessary health-checks for 
the dependent components automatically
![healthchecks-dependent-checks](./Images/healthchecks-dependent-checks.PNG)

```csharp
var services = builder.Services;

// disable these codes and the health-checks dashboard will remove the related checks on UI too
//builder.AddRedisDistributedCache(Constants.RedisCache);
//builder.AddRedisOutputCache(Constants.RedisCache);

builder.AddSqlServerDbContext<EmployeeDbContext>(Constants.EmployeesDB,
sqlEFCoreOpts =>
{
	//get connection string from appsettings.json
	//sqlEFCoreOpts.ConnectionString = @"Data Source=IDL-LT-127\SQLEXPRESS;Database=employees-sqldb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
	sqlEFCoreOpts.DisableRetry = true;
},
dbContextOpts =>
{
	//dbContextOpts.UseModel(Infrastructure.CompiledModels.EmployeeDbContextModel.Instance);
	dbContextOpts.EnableSensitiveDataLogging();
	dbContextOpts.EnableDetailedErrors();
	dbContextOpts.LogTo(Console.WriteLine); // Log to console
});
```

![healthchecks-dependent-check-1](./Images/healthchecks-dependent-check-1.PNG)