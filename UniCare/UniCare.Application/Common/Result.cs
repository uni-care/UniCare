namespace UniCare.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? ErrorMessage { get; }
    public string ErrorCode { get; }
    public int StatusCode { get; }
    public IEnumerable<string> ValidationErrors { get; }

    private Result(
        bool isSuccess,
        T? data,
        string? errorMessage,
        string errorCode,
        int statusCode,
        IEnumerable<string>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        StatusCode = statusCode;
        ValidationErrors = validationErrors ?? [];
    }

    public static Result<T> Success(T data)
        => new(true, data, null, string.Empty, 200);
    public static Result<T> Created(T data)
        => new(true, data, null, string.Empty, 201);
    public static Result<T> Failure(string errorMessage, int statusCode = 400, string errorCode = "BAD_REQUEST")
        => new(false, default, errorMessage, errorCode, statusCode);
    public static Result<T> NotFound(string errorMessage)
        => new(false, default, errorMessage, "NOT_FOUND", 404);
    public static Result<T> Unauthorized(string errorMessage = "Unauthorized access.")
        => new(false, default, errorMessage, "UNAUTHORIZED", 401);
    public static Result<T> Forbidden(string errorMessage = "Access denied.")
        => new(false, default, errorMessage, "FORBIDDEN", 403);
    public static Result<T> ValidationFailure(IEnumerable<string> errors)
        => new(false, default, "One or more validation errors occurred.", "VALIDATION_ERROR", 422, errors);
    public static Result<T> Conflict(string errorMessage)
        => new(false, default, errorMessage, "CONFLICT", 409);
}
