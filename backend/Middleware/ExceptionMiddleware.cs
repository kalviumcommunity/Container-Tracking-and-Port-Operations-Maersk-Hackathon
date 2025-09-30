using Backend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            
            var response = ApiResponse<object>.Fail(ex.Message);
            
            switch (ex)
            {
                case KeyNotFoundException _:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case ArgumentException _:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case UnauthorizedAccessException _:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = ApiResponse<object>.Fail("An unexpected error occurred. Please try again later.");
                    break;
            }
            
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}