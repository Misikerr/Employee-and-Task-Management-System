using EmployeeTaskManagement.Exceptions;
using System.Text.Json;
using AppException = EmployeeTaskManagement.Exceptions.ApplicationException;
using AppValidationException = EmployeeTaskManagement.Exceptions.ValidationException;

namespace EmployeeTaskManagement.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow
            };

            if (exception is AppException appEx)
            {
                context.Response.StatusCode = appEx.StatusCode;
                response.StatusCode = appEx.StatusCode;
                response.Message = appEx.Message;
                response.ErrorCode = appEx.ErrorCode;

                if (exception is AppValidationException validationEx)
                {
                    response.ValidationErrors = validationEx.Errors;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "An internal server error occurred. Please try again later.";
                response.ErrorCode = "INTERNAL_SERVER_ERROR";
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsJsonAsync(response, options);
        }
    }
}
