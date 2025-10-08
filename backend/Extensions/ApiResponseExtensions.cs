using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Extensions
{
    /// <summary>
    /// Extensions for ApiResponse class
    /// </summary>
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Create ApiResponse from ActionResult
        /// </summary>
        public static ActionResult<ApiResponse<T>> ToApiResponse<T>(this ActionResult<T> result, string? message = "")
        {
            if (result.Result is OkObjectResult okResult && okResult.Value is T value)
            {
                return new ActionResult<ApiResponse<T>>(ApiResponse<T>.OkWithData(value, message ?? "Operation completed successfully"));
            }
            
            return new ActionResult<ApiResponse<T>>(ApiResponse<T>.Fail(message ?? "Operation failed"));
        }
        
        /// <summary>
        /// Create successful ApiResponse from value
        /// </summary>
        public static ActionResult<ApiResponse<T>> ToSuccessResponse<T>(this T value, string? message = "")
        {
            return new OkObjectResult(ApiResponse<T>.OkWithData(value, message ?? "Operation completed successfully"));
        }
        
        /// <summary>
        /// Create error ApiResponse
        /// </summary>
        public static ActionResult<ApiResponse<object>> ToErrorResponse(this string errorMessage, int statusCode = 400)
        {
            var response = ApiResponse<object>.Fail(errorMessage);
            
            return statusCode switch
            {
                404 => new NotFoundObjectResult(response),
                401 => new UnauthorizedObjectResult(response),
                403 => new ForbidResult(),
                _ => new BadRequestObjectResult(response)
            };
        }
    }
}
