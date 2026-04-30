using System.Net;
using System.Text.Json;
using UniCare.Application.Common;
using FluentValidation;

namespace UniCare.Api.Middelware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation exception occurred.");
                await WriteErrorAsync(
                    context,
                    HttpStatusCode.UnprocessableEntity,
                    "One or more validation errors occurred.",
                    "VALIDATION_ERROR",
                    ex.Errors.Select(e => e.ErrorMessage));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access.");
                await WriteErrorAsync(context, HttpStatusCode.Unauthorized, ex.Message, "UNAUTHORIZED");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await WriteErrorAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.",
                    "INTERNAL_SERVER_ERROR");
            }
        }

        private static async Task WriteErrorAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message,
            string errorCode,
            IEnumerable<string>? errors = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Fail(message, errorCode, errors);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }

    /// <summary>Extension method to register <see cref="ExceptionHandlingMiddleware"/>.</summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

}
