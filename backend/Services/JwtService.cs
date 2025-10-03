using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;

namespace Backend.Services
{
    /// <summary>
    /// Service for JWT token operations
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generate JWT token for a user
        /// </summary>
        string GenerateToken(User user, List<string> roles, List<string> permissions);

        /// <summary>
        /// Validate JWT token and extract claims
        /// </summary>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Get token expiration time
        /// </summary>
        DateTime GetTokenExpiration();
    }

    /// <summary>
    /// JWT service implementation
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // Get JWT key from configuration or environment variables
            var jwtKeyFromConfig = configuration["Jwt:Key"];
            var jwtKeyFromEnv = Environment.GetEnvironmentVariable("JWT_KEY");
            _jwtKey = string.IsNullOrEmpty(jwtKeyFromConfig) ? jwtKeyFromEnv : jwtKeyFromConfig;
            
            if (string.IsNullOrEmpty(_jwtKey))
            {
                throw new ArgumentNullException("JWT_KEY is not configured in either appsettings.json or environment variables");
            }
            
            _jwtIssuer = configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ContainerTrackingAPI";
            _jwtAudience = configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ContainerTrackingClients";
            _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? "60");
        }

        /// <summary>
        /// Generate JWT token for a user with roles and permissions
        /// </summary>
        public string GenerateToken(User user, List<string> roles, List<string> permissions)
        {
            // Convert Base64 JWT key to bytes
            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(_jwtKey);
            }
            catch (FormatException)
            {
                // If it's not Base64, treat as regular string (for backward compatibility)
                keyBytes = Encoding.UTF8.GetBytes(_jwtKey);
            }
            
            var key = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permission claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            // Add port-specific claims if user is assigned to a port
            if (user.PortId.HasValue)
            {
                claims.Add(new Claim("PortId", user.PortId.Value.ToString()));
            }

            // Add department claim if available
            if (!string.IsNullOrEmpty(user.Department))
            {
                claims.Add(new Claim("Department", user.Department));
            }

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Validate JWT token and return claims principal
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get token expiration time
        /// </summary>
        public DateTime GetTokenExpiration()
        {
            return DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes);
        }
    }

    /// <summary>
    /// Extension methods for ClaimsPrincipal
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get user ID from claims
        /// </summary>
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst("UserId") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        /// <summary>
        /// Get username from claims
        /// </summary>
        public static string GetUsername(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Get user email from claims
        /// </summary>
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Get user full name from claims
        /// </summary>
        public static string GetFullName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("FullName")?.Value ?? string.Empty;
        }

        /// <summary>
        /// Get assigned port ID from claims
        /// </summary>
        public static int? GetPortId(this ClaimsPrincipal principal)
        {
            var portIdClaim = principal.FindFirst("PortId")?.Value;
            return int.TryParse(portIdClaim, out var portId) ? portId : null;
        }

        /// <summary>
        /// Get user department from claims
        /// </summary>
        public static string? GetDepartment(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("Department")?.Value;
        }

        /// <summary>
        /// Get user roles from claims
        /// </summary>
        public static List<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }

        /// <summary>
        /// Get user permissions from claims
        /// </summary>
        public static List<string> GetPermissions(this ClaimsPrincipal principal)
        {
            return principal.FindAll("permission").Select(c => c.Value).ToList();
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        public static bool HasPermission(this ClaimsPrincipal principal, string permission)
        {
            return principal.FindAll("permission").Any(c => c.Value == permission);
        }

        /// <summary>
        /// Check if user has any of the specified permissions
        /// </summary>
        public static bool HasAnyPermission(this ClaimsPrincipal principal, params string[] permissions)
        {
            var userPermissions = principal.GetPermissions();
            return permissions.Any(p => userPermissions.Contains(p));
        }

        /// <summary>
        /// Check if user has all of the specified permissions
        /// </summary>
        public static bool HasAllPermissions(this ClaimsPrincipal principal, params string[] permissions)
        {
            var userPermissions = principal.GetPermissions();
            return permissions.All(p => userPermissions.Contains(p));
        }

        /// <summary>
        /// Check if user has specific role
        /// </summary>
        public static bool IsInRole(this ClaimsPrincipal principal, string role)
        {
            return principal.FindAll(ClaimTypes.Role).Any(c => c.Value == role);
        }

        /// <summary>
        /// Check if user has any of the specified roles
        /// </summary>
        public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles)
        {
            var userRoles = principal.GetRoles();
            return roles.Any(r => userRoles.Contains(r));
        }
    }
}