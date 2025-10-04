using Microsoft.AspNetCore.Http;

namespace Backend.Middleware
{
    /// <summary>
    /// Middleware to add security headers for production
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            var headers = context.Response.Headers;

            // Prevent clickjacking
            headers["X-Frame-Options"] = "DENY";

            // Prevent MIME type sniffing
            headers["X-Content-Type-Options"] = "nosniff";

            // Enable XSS protection
            headers["X-XSS-Protection"] = "1; mode=block";

            // Strict Transport Security (HTTPS only)
            if (context.Request.IsHttps)
            {
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            }

            // Content Security Policy
            headers["Content-Security-Policy"] = 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "connect-src 'self'; " +
                "font-src 'self'; " +
                "object-src 'none'; " +
                "media-src 'self'; " +
                "frame-src 'none';";

            // Referrer Policy
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Permissions Policy
            headers["Permissions-Policy"] = 
                "camera=(), " +
                "microphone=(), " +
                "geolocation=(), " +
                "payment=(), " +
                "usb=(), " +
                "magnetometer=(), " +
                "accelerometer=(), " +
                "gyroscope=()";

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to add security headers middleware
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}