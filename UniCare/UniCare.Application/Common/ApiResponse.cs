namespace UniCare.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }

    public IEnumerable<string>? Errors { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, string errorCode = "ERROR", IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, ErrorCode = errorCode, Errors = errors };
    public static ApiResponse<T> FromResult(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data!, "Success");

        return new ApiResponse<T>
        {
            Success = false,
            Message = result.ErrorMessage,
            ErrorCode = result.ErrorCode,
            Errors = result.ValidationErrors.Any() ? result.ValidationErrors : null
        };
    }
}
