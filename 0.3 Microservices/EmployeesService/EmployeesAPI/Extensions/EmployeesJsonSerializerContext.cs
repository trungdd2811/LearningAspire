

namespace Employees.API.Extensions
{
    [JsonSerializable(typeof(Employee))]
    [JsonSerializable(typeof(ApiResponse<Employee>))]
    [JsonSerializable(typeof(IEnumerable<Employee>))]
    [JsonSerializable(typeof(ApiResponse<IEnumerable<Employee>>))]
    internal partial class EmployeesJsonSerializerContext : CustomJsonSerializerContext
    {
    }
}
