using Employees.Domain.AggregateModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeesController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            // Log the exception details
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with Id = {id} not found.");
            }
            return Ok(employee);
        }
        catch (Exception ex)
        {
            // Log the exception details
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee employee)
    {
        if (employee == null)
        {
            return BadRequest("Employee is null.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.UnitOfWork.SaveEntitiesAsync();
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            // Log the exception details
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new employee record");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Employee employee)
    {
        if (employee == null || id != employee.Id)
        {
            return BadRequest("Employee is null or ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating employee record");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
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
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting employee record");
        }
    }
}
