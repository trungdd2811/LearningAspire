using Employees.Domain.AggregateModel;
using LearningAspire.Commons.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee> GetByIdAsync(Guid id);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task DeleteAsync(Guid id);
}

