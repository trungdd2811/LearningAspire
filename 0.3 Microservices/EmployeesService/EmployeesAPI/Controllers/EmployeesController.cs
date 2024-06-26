[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
	private readonly IEmployeeRepository _employeeRepository;

	public EmployeesController(IEmployeeRepository employeeRepository)
	{
		_employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
	}
	[OutputCache(Duration = 10)]
	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		try
		{
			var employees = await _employeeRepository.GetAllAsync();
			var listEmp = new List<EmployeeDTO>();
			foreach (var e in employees)
			{
				listEmp.Add(e.ToDTO());
			}
			return Ok(new ApiResponse<IEnumerable<EmployeeDTO>>(listEmp));
		}
		catch (Exception ex)
		{
			// Log the exception details
			var emp = new Employee("test");
			var api = new ApiResponse<Employee>(emp, "no employee");
			var result = JsonSerializer.Serialize(api, EmployeesJsonSerializerContext.Default.ApiResponseEmployee);
			//return StatusCode(StatusCodes.Status500InternalServerError, result);
			return StatusCode(StatusCodes.Status500InternalServerError, api);

		}
	}

	[OutputCache(Duration = 10)]
	[HttpGet("GetAllWithoutDTO")]
	public async Task<IActionResult> GetAllWithoutDTO()
	{
		try
		{
			var employees = await _employeeRepository.GetAllAsync();
			return Ok(new ApiResponse<IEnumerable<Employee>>(employees));
		}
		catch (Exception ex)
		{
			// Log the exception details
			var emp = new Employee("test");
			var api = new ApiResponse<Employee>(emp, "no employee");
			var result = JsonSerializer.Serialize(api, EmployeesJsonSerializerContext.Default.ApiResponseEmployee);
			//return StatusCode(StatusCodes.Status500InternalServerError, result);
			return StatusCode(StatusCodes.Status500InternalServerError, api);

		}
	}

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

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] Employee employee)
	{
		if (employee == null)
		{
			return BadRequest(new ApiResponse<string>("Employee is null."));
		}

		if (!ModelState.IsValid)
		{
			return BadRequest(new ApiResponse<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>(ModelState));
		}

		try
		{
			await _employeeRepository.AddAsync(employee);
			await _employeeRepository.UnitOfWork.SaveEntitiesAsync();
			return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new ApiResponse<Employee>(employee));
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>("Error creating new employee record"));
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, [FromBody] Employee employee)
	{
		if (employee == null || id != employee.Id)
		{
			return BadRequest(new ApiResponse<string>("Employee is null or ID mismatch."));
		}

		if (!ModelState.IsValid)
		{
			return BadRequest(new ApiResponse<Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary>(ModelState));
		}

		try
		{
			await _employeeRepository.UpdateAsync(employee);
			await _employeeRepository.UnitOfWork.SaveEntitiesAsync();
			return NoContent();
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>("Error updating employee record"));
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			await _employeeRepository.DeleteAsync(id);
			await _employeeRepository.UnitOfWork.SaveEntitiesAsync();
			return NoContent();
		}
		catch (Exception ex)
		{
			// Log the exception details
			return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>("Error deleting employee record"));
		}
	}
}
