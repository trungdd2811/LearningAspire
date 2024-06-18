- How to serialize a new class in AOT compliant application

   var emp = new Employee("Error retrieving data from the database");
   var api = new ApiResponse<Employee>(emp, "no employee");
   var result = JsonSerializer.Serialize(api, EmployeesJsonSerializerContext.Default.ApiResponseEmployee);