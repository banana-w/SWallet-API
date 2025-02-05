using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SWallet.Repository.Payload.ExceptionModels;
using System.Diagnostics;
using System.Text.Json;

namespace SWallet.Repository.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (ex)
            {
                case ApiException apiEx:
                    context.Response.StatusCode = apiEx.StatusCode;
                    errorResponse.Message = apiEx.Message;
                    errorResponse.ErrorCode = apiEx.ErrorCode;
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    errorResponse.Message = argEx.Message;
                    errorResponse.ErrorCode = "INVALID_ARGUMENT";
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    errorResponse.Message = "Unauthorized access";
                    errorResponse.ErrorCode = "UNAUTHORIZED";
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    errorResponse.Message = "Resource not found";
                    errorResponse.ErrorCode = "NOT_FOUND";
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    errorResponse.Message = "An internal server error occurred";
                    errorResponse.ErrorCode = "INTERNAL_ERROR";
                    errorResponse.Details = ex.Message;
                    _logger.LogError(ex, "An unhandled exception occurred");
                    break;
            }

            var errorResponseJson = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(errorResponseJson);
        }
    }
}
