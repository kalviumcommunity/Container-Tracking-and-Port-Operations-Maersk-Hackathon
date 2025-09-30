using Backend.Middleware;

namespace Backend.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds global exception handling middleware to the application
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}