﻿using LearningAspire.Commons.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Employees.Domain.AggregateModel
{
	/// <summary>
	/// Represents an employee entity.
	/// </summary>
	public class Employee : Entity, IAggregateRoot
	{
		[Required]
		/// <summary>
		/// Gets the identity GUID of the employee.
		/// </summary>
		public override int Id { get; protected set; }

		/// <summary>
		/// Gets the name of the employee.
		/// </summary>
		public string Name { get; private set; }

		private List<Employee> _directReports;
		private List<Employee> _managers;

		/// <summary>
		/// Gets the collection of direct reports of the employee.
		/// </summary>
		public IReadOnlyCollection<Employee> DirectReports => _directReports?.AsReadOnly();

		/// <summary>
		/// Gets the collection of managers of the employee.
		/// </summary>
		public IReadOnlyCollection<Employee> Managers => _managers?.AsReadOnly();

		/// <summary>
		/// Initializes a new instance of the <see cref="Employee"/> class.
		/// </summary>
		protected Employee()
		{
			_directReports = new List<Employee>();
			_managers = new List<Employee>();

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Employee"/> class with the specified identity GUID and name.
		/// </summary>
		/// <param name="identityGuid">The identity GUID of the employee.</param>
		/// <param name="name">The name of the employee.</param>
		public Employee(string name) : this()
		{
			Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
		}

		/// <summary>
		/// Adds an employee to the direct reports collection.
		/// </summary>
		/// <param name="employee">The employee to add.</param>
		public void AddDirectReport(Employee employee)
		{
			var existing = _directReports.SingleOrDefault(d => d.Id == employee.Id);
			if (existing != null)
			{
				throw new InvalidOperationException("Employee already exists in direct reports.");
			}
			_directReports.Add(employee);
		}

		/// <summary>
		/// Removes an employee from the direct reports collection.
		/// </summary>
		/// <param name="employee">The employee to remove.</param>
		public void RemoveDirectReport(Employee employee)
		{
			_directReports.Remove(employee);
		}

		/// <summary>
		/// Adds an employee to the managers collection.
		/// </summary>
		/// <param name="employee">The employee to add.</param>
		public void AddManager(Employee employee)
		{
			var existing = _managers.SingleOrDefault(m => m.Id == employee.Id);
			if (existing != null)
			{
				throw new InvalidOperationException("Employee already exists in managers.");
			}
			_managers.Add(employee);
		}

		/// <summary>
		/// Removes an employee from the managers collection.
		/// </summary>
		/// <param name="employee">The employee to remove.</param>
		public void RemoveManager(Employee employee)
		{
			_managers.Remove(employee);
		}

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

	//generate dto object for this file
	public class EmployeeDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<EmployeeDTO> DirectReports { get; set; } = new List<EmployeeDTO>();
		public List<EmployeeDTO> Managers { get; set; } = new List<EmployeeDTO>();
	}
}
