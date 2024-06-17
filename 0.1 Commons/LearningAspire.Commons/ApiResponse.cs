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

