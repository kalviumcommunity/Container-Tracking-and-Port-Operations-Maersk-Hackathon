using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The main data payload
        /// </summary>
        public T? Data { get; set; }
        
        /// <summary>
        /// Error message if an error occurred
        /// </summary>
        public string? Message { get; set; }
        
        /// <summary>
        /// Validation errors if applicable
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }
        
        /// <summary>
        /// Create a successful response with data
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <returns>A successful response with data</returns>
        public static ApiResponse<T> Ok(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = null
            };
        }
        
        /// <summary>
        /// Create a successful response with a message
        /// </summary>
        /// <param name="message">The success message</param>
        /// <returns>A successful response with a message</returns>
        public static ApiResponse<T> OkWithMessage(string message)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = default
            };
        }
        
        /// <summary>
        /// Create a successful response with data and a message
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">The success message</param>
        /// <returns>A successful response with data and a message</returns>
        public static ApiResponse<T> OkWithData(T data, string message)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }
        
        /// <summary>
        /// Create a failed response with an error message
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>A failed response with an error message</returns>
        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
        
        /// <summary>
        /// Create a failed response with validation errors
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="errors">List of validation errors</param>
        /// <returns>A failed response with validation errors</returns>
        public static ApiResponse<T> Fail(string message, List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors,
                Data = default
            };
        }
    }
}