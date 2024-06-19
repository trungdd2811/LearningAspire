# Lessons learned from creating NativeAOT applications


## 1. Reflection issue: JIT (Just-In-Time) vs AOT (Ahead-Of-Time) compilation

When using NativeAOT, you will have to deal with reflection issues.
When creating NativeAOT applications, one of the primary issues encountered is related to reflection. 
Reflection is a powerful feature in .NET that allows programs to inspect and modify their own structure and behavior at runtime. 
However, this dynamic capability (which is supported by JIT (Just-In-Time) compilation) poses challenges for NativeAOT (Ahead-Of-Time compilation),
which significantly relies on static analysis to compile .NET applications into native binaries. 
Here are the key issues with reflection in the context of NativeAOT applications:
1.	Limited Runtime Metadata: NativeAOT aims to reduce the size of the final binary and improve performance. To achieve this, it removes unused metadata and code paths, which are often required for reflection. This means that at runtime, the application may not have enough information to perform reflection operations, such as inspecting types, members, attributes, or creating instances dynamically.
2.	Static Analysis Limitations: NativeAOT uses static analysis to understand what parts of the code are used and how. However, reflection introduces uncertainty because it can dynamically access types and members based on strings or other runtime data. This makes it difficult for the static analysis to accurately predict and include all necessary metadata and code paths in the compiled binary.
3.	Runtime Code Generation: Some reflection scenarios involve generating code at runtime (e.g., through Reflection.Emit). NativeAOT fundamentally does not support runtime code generation, as all code must be compiled ahead of time. This limitation affects scenarios that rely on dynamic proxy generation, serialization libraries, and other dynamic features.

(please ask Github Copilot the question "please tell me the Reflection issue when creating nativeAOT applications" 
to make it more clear)

## 2. Using System.Text.Json instead of Newtonsoft.Json

When working with NativeAOT applications, it is recommended to use `System.Text.Json` instead of `Newtonsoft.Json` for JSON serialization and deserialization.

1. How to serialize and deserialize generic types with System.Text.Json

```csharp
#example
public class ApiResponse<T>
{
	public bool Success { get; set; }
	public string Message { get; set; }
	public T Data { get; set; }
	public object Errors { get; set; }

	public ApiResponse(T data, string message = null)
	{
		Success = true;
		Message = message;
		Data = data;
		Errors = null;
	}

	public ApiResponse(string message, object errors)
	{
		Success = false;
		Message = message;
		Data = default;
		Errors = errors;
	}
}

```

* Create a custom JsonSerializerContext and specify the types to be serialized/deserialized:

```csharp
	[JsonSerializable(typeof(Employee))]
	[JsonSerializable(typeof(ApiResponse<Employee>))]
	[JsonSerializable(typeof(IEnumerable<Employee>))]
	[JsonSerializable(typeof(ApiResponse<IEnumerable<Employee>>))]
	internal partial class EmployeesJsonSerializerContext : JsonSerializerContext
	{
	}
```
* Add this context to JsonOptions globally when call AddControllers if you want asp.net core will deserialize/serialize 
the object automatically when return the object in the controller
```csharp
	services.AddControllers()
			.AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, EmployeesJsonSerializerContext.Default);
			});
```
* then you can use it in the controller 
```csharp
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Employee>>> Get()
	{
		var employees = await _employeeService.GetEmployeesAsync();
		return Ok(new ApiResponse<IEnumerable<Employee>>(employees));
	}
```

* or use it directly as below
```csharp
	 var json = JsonSerializer.Serialize(new ApiResponse<IEnumerable<Employee>>(employees), 
				EmployeesJsonSerializerContext.Default.ApiResponseIEnumerableEmployee);
```