# Lessons learned from creating Aspire App Host (Orchestration)


## 1. Project References
There are 2 kinds of project references when adding them to Aspire App Host (Orchestration) project:
1. **Project Reference**: You have to edit the project file manually . This is the normal project reference that is added to the project. This is used when the project is part of the solution.
2. **Project Resources**: It is a default setting. A resource represents a part of an application whether it be a .NET project, container, or executable, or some other resource like a database, cache, or cloud service (such as a storage service)

```xml
  <ItemGroup>
	<ProjectReference Include="..\..\0.1 Commons\LearningAspire.Commons\LearningAspire.Commons.csproj" IsAspireProjectResource="false" />
	<ProjectReference Include="..\..\0.3 Microservices\EmployeesService\EmployeesAPI\Employees.API.csproj" />
	<ProjectReference Include="..\..\0.3 Microservices\LearningAspire.ApiService\LearningAspire.ApiService.csproj" />
	<ProjectReference Include="..\..\0.4 UIs\LearningAspire.Web\LearningAspire.Web.csproj" />
  </ItemGroup>
```
## 2. Persist .NET Aspire project data using volumes
Remember to check if we grant the necessary permissions (WRITE) to the containers to access the folders.
```csharp
#sqlServer.WithBindMount("VolumeMount.AppHost-sql-data", "/var/opt/mssql");
#or
sqlServer.WithDataVolume(Constants.EmployeesSQLServer);
```
## 3. How to access the SQL Server from the container locally

1. We should set a specific port when adding SQL Server component to the Aspire app host. 
	```csharp
	var sqlPassword = builder.AddParameter("sql-password", secret: true);
	var sqlServer = builder.AddSqlServer(Constants.EmployeesSQLServer, password: sqlPassword, 1443);
	```

2. How to get the connection string from the container:

	You can use the following code to get the connection string from the container
	```csharp
	_context.Database.GetConnectionString(); #_context is your dbcontext
	#output: "Server=127.0.0.1,1443;User ID=sa;Password=Trung123456;TrustServerCertificate=true;Database=employees-sqldb"
	``` 

	Or you can see the following images to get the necessary information which is used to connect the database
	
	![aspire-host-sqlserver-port.PNG](0.6%20Lessons%20Learnt/Images/aspire-host-sqlserver-port.PNG)

	![aspire-host-sqlserver-connect.PNG](0.6%20Lessons%20Learnt/Images/aspire-host-sqlserver-connect.PNG)


## 4. How the Aspire App Host (Orchestration) works basically
* Aspire App Host will create docker containers for most of the components in the project 
* It will handle connections/relationships/instances between these components automatically. These information is stored in enviroment variables. 
If you want, you can also modify them manually in code. Please read the official document of Microsoft for more information.


## Continue writing here ...

