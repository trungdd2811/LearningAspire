# Lessons learned when using System.Text.Json

## Circular references

System.Text.Json does not support circular references. If you try to serialize an object that has a circular reference, 
you will get a `System.Text.Json.JsonException` with the message `A possible object cycle was detected which is not supported.`.

Example:

```csharp
	public class Employee
	{
		public int Id { get; set; }
		public string Name { get; set; }

		//Circular references
		public List<Employee> DirectReports { get; set; } = new List<Employee>();
		public List<Employee> Managers { get; set; } = new List<Employee>();
	}

	public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
	{
		public void Configure(EntityTypeBuilder<Employee> builder)
		{
			// Set the table name
			builder.ToTable("Employees");

			// Set the primary key
			builder.HasKey(e => e.Id);

			builder.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(200);

			// Configure the relationships
			// Assuming a self-referencing many-to-many relationship for Managers and DirectReports
			// This requires a join table. Here's an example configuration:

			builder.HasMany(e => e.DirectReports)
				.WithMany(e => e.Managers)
				.UsingEntity<Dictionary<string, object>>(
					"EmployeeHierarchy",
					j => j.HasOne<Employee>().WithMany().HasForeignKey("DirectReportId"),
					j => j.HasOne<Employee>().WithMany().HasForeignKey("ManagerId"),
					j =>
					{
						j.HasKey("DirectReportId", "ManagerId");
						j.ToTable("EmployeeHierarchies"); // Name of the join table
					});
		}
	}

```

There are 2 popular ways to handle circular references:

1. **Using DTO (Data Transfer Objects)**: This is the most common way to handle circular references. You create a DTO class that contains only the properties you want to serialize. You then map the properties from the original object to the DTO object and serialize the DTO object.

```csharp
	public class EmployeeDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<EmployeeDTO> DirectReports { get; set; } = new List<EmployeeDTO>();
		public List<EmployeeDTO> Managers { get; set; } = new List<EmployeeDTO>();
	}

	public class Employee
	{
		// Other codes

		public EmployeeDTO ToDTO()
		{
			var emp = new EmployeeDTO();
			emp.Id = this.Id;
			emp.Name = this.Name;
			foreach (var employee in _managers)
			{
				var emp1 = new EmployeeDTO();
				emp1.Id = employee.Id;
				emp1.Name = employee.Name;
				emp.Managers.Add(emp1);
			}

			foreach (var employee in _directReports)
			{
				var emp1 = new EmployeeDTO();
				emp1.Id = employee.Id;
				emp1.Name = employee.Name;
				emp.DirectReports.Add(emp1);
			}
			return emp;
		}
	}

	/// code in controller

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		try
		{
			var employee = await _employeeRepository.GetByIdAsync(id);
			if (employee == null)
			{
				return NotFound(new ApiResponse<string>($"Employee with Id = {id} not found."));
			}
			return Ok(new ApiResponse<EmployeeDTO>(employee.ToDTO()));
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>("Error retrieving data from the database"));
		}
	}
```

then returned JSON will be 
```json
{
    "Success": true,
    "Message": null,
    "Data": {
        "Id": 1,
        "Name": "Trung",
        "DirectReports": [
            {
                "Id": 2,
                "Name": "Hoa",
                "DirectReports": [],
                "Managers": []
            }
        ],
        "Managers": [
            {
                "Id": 3,
                "Name": "Roeland",
                "DirectReports": [],
                "Managers": []
            }
        ]
    },
    "Errors": null
}
```

2. **Using ReferenceHandler.Preserve**: This is a new feature in .NET 5.0. You can use the `ReferenceHandler.Preserve` option to handle circular references. This option will preserve the reference to the object when it is serialized. 


```csharp
	var options = new JsonSerializerOptions
	{
		ReferenceHandler = ReferenceHandler.Preserve
	};

	var json = JsonSerializer.Serialize(employee, options);

	//or
	services.AddControllers()
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
			options.JsonSerializerOptions.PropertyNamingPolicy = null;

		});

```

then returned JSON will be below. $id, $ref are used to represent the circular references.
We should to write a custom json parser to handle this case.

```json
{
    "$id": "1",
    "Success": true,
    "Message": null,
    "Data": {
        "$id": "2",
        "Id": 1,
        "Name": "Trung",
        "DirectReports": {
            "$id": "3",
            "$values": [
                {
                    "$id": "4",
                    "Id": 2,
                    "Name": "Hoa",
                    "DirectReports": {
                        "$id": "5",
                        "$values": []
                    },
                    "Managers": {
                        "$id": "6",
                        "$values": [
                            {
                                "$ref": "2"
                            }
                        ]
                    },
                    "DomainEvents": null
                }
            ]
        },
        "Managers": {
            "$id": "7",
            "$values": [
                {
                    "$id": "8",
                    "Id": 3,
                    "Name": "Roeland",
                    "DirectReports": {
                        "$id": "9",
                        "$values": [
                            {
                                "$ref": "2"
                            }
                        ]
                    },
                    "Managers": {
                        "$id": "10",
                        "$values": []
                    },
                    "DomainEvents": null
                }
            ]
        },
        "DomainEvents": null
    },
    "Errors": null
}
```