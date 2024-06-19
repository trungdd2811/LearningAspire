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

1. We should set a specic port when adding SQL Server component to the Aspire app host. 
    ```csharp
    var sqlPassword = builder.AddParameter("sql-password", secret: true);
    var sqlServer = builder.AddSqlServer(Constants.EmployeesSQLServer, password: sqlPassword, 1443);
    ```
2. How to get the connection string from the container:

    You can use the following code to get the connection string from the container
	```csharp
	_context.Database.GetConnectionString(); #_context is your dbcontext_
    #output: "Server=127.0.0.1,1443;User ID=sa;Password=Trung123456;TrustServerCertificate=true;Database=employees-sqldb"
	``` 

    Or you can see the following images to get the necessary information which is used to connect the database

    ![Aspire Host SQL Server Port](./Images/aspire-host-sqlserver-port.PNG)
    ![Aspire Host SQL Server Connect](./Images/aspire-host-sqlserver-connect.PNG)

## See also