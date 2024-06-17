How to add a migration

- go to the project root directory (Employees.Infrastructure)
- set this project to startup project
- go to Package Manager Console (Tools -> NuGet Package Manager -> Package Manager Console)
- run the command `Add-Migration MigrationName`


- to generate the SQL script for the migration, 
run the command `Script-Migration -From PreviousMigrationName -To MigrationName`

Noted: Please implement DBContextFactory to avoid the error "No parameterless constructor defined for this object."
