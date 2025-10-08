using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Backend.Attributes
{
    /// <summary>
    /// Authorization attribute that requires specific permissions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;
        private readonly bool _requireAll;

        /// <summary>
        /// Require specific permissions
        /// </summary>
        /// <param name="permission">Required permission</param>
        /// <param name="requireAll">If true, user must have all permissions. If false, user needs at least one.</param>
        public RequirePermissionAttribute(string permission, bool requireAll = false)
        {
            _permissions = new[] { permission };
            _requireAll = requireAll;
        }

        /// <summary>
        /// Require multiple permissions
        /// </summary>
        /// <param name="permissions">Required permissions</param>
        /// <param name="requireAll">If true, user must have all permissions. If false, user needs at least one.</param>
        public RequirePermissionAttribute(string[] permissions, bool requireAll = false)
        {
            _permissions = permissions;
            _requireAll = requireAll;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get user permissions from claims
            var userPermissions = user.FindAll("permission").Select(c => c.Value).ToList();

            // Check permission requirements
            bool hasRequiredPermissions;
            if (_requireAll)
            {
                // User must have ALL specified permissions
                hasRequiredPermissions = _permissions.All(p => userPermissions.Contains(p));
            }
            else
            {
                // User must have AT LEAST ONE of the specified permissions
                hasRequiredPermissions = _permissions.Any(p => userPermissions.Contains(p));
            }

            if (!hasRequiredPermissions)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    /// <summary>
    /// Authorization attribute that requires specific roles
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;
        private readonly bool _requireAll;

        /// <summary>
        /// Require specific role
        /// </summary>
        /// <param name="role">Required role</param>
        public RequireRoleAttribute(string role)
        {
            _roles = new[] { role };
            _requireAll = false;
        }

        /// <summary>
        /// Require multiple roles
        /// </summary>
        /// <param name="roles">Required roles</param>
        /// <param name="requireAll">If true, user must have all roles. If false, user needs at least one.</param>
        public RequireRoleAttribute(string[] roles, bool requireAll = false)
        {
            _roles = roles;
            _requireAll = requireAll;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get user roles from claims
            var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Check role requirements
            bool hasRequiredRoles;
            if (_requireAll)
            {
                // User must have ALL specified roles
                hasRequiredRoles = _roles.All(r => userRoles.Contains(r));
            }
            else
            {
                // User must have AT LEAST ONE of the specified roles
                hasRequiredRoles = _roles.Any(r => userRoles.Contains(r));
            }

            if (!hasRequiredRoles)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    /// <summary>
    /// Authorization attribute that requires access to a specific port
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePortAccessAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _portIdParameterName;
        private readonly bool _allowGlobalAccess;

        /// <summary>
        /// Require access to a specific port
        /// </summary>
        /// <param name="portIdParameterName">Name of the route/query parameter containing the port ID</param>
        /// <param name="allowGlobalAccess">If true, users with global access permissions can access any port</param>
        public RequirePortAccessAttribute(string portIdParameterName = "portId", bool allowGlobalAccess = true)
        {
            _portIdParameterName = portIdParameterName;
            _allowGlobalAccess = allowGlobalAccess;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get the port ID from the request
            var request = context.HttpContext.Request;
            string? portIdString = null;

            // Try to get port ID from route values
            if (context.RouteData.Values.ContainsKey(_portIdParameterName))
            {
                portIdString = context.RouteData.Values[_portIdParameterName]?.ToString();
            }
            // Try to get port ID from query parameters
            else if (request.Query.ContainsKey(_portIdParameterName))
            {
                portIdString = request.Query[_portIdParameterName];
            }

            if (string.IsNullOrEmpty(portIdString) || !int.TryParse(portIdString, out var requestedPortId))
            {
                context.Result = new BadRequestObjectResult("Invalid or missing port ID");
                return;
            }

            // Check if user has global access (Admin role or global permissions)
            if (_allowGlobalAccess)
            {
                var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                var userPermissions = user.FindAll("permission").Select(c => c.Value).ToList();

                // Allow if user is Admin or has global access permissions
                if (userRoles.Contains("Admin") || 
                    userPermissions.Contains("GlobalPortAccess") ||
                    userPermissions.Contains("ManageAllPorts"))
                {
                    return;
                }
            }

            // Check if user is assigned to the requested port
            var userPortIdClaim = user.FindFirst("PortId");
            if (userPortIdClaim != null && int.TryParse(userPortIdClaim.Value, out var userPortId))
            {
                if (userPortId == requestedPortId)
                {
                    return; // User has access to their assigned port
                }
            }

            // User doesn't have access to this port
            context.Result = new ForbidResult();
        }
    }

    /// <summary>
    /// Authorization attribute that requires the user to access only their own data
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireOwnershipAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _userIdParameterName;
        private readonly bool _allowAdminOverride;

        /// <summary>
        /// Require user to access only their own data
        /// </summary>
        /// <param name="userIdParameterName">Name of the route/query parameter containing the user ID</param>
        /// <param name="allowAdminOverride">If true, users with Admin role can access any user's data</param>
        public RequireOwnershipAttribute(string userIdParameterName = "userId", bool allowAdminOverride = true)
        {
            _userIdParameterName = userIdParameterName;
            _allowAdminOverride = allowAdminOverride;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get the user ID from the request
            var request = context.HttpContext.Request;
            string? userIdString = null;

            // Try to get user ID from route values
            if (context.RouteData.Values.ContainsKey(_userIdParameterName))
            {
                userIdString = context.RouteData.Values[_userIdParameterName]?.ToString();
            }
            // Try to get user ID from query parameters
            else if (request.Query.ContainsKey(_userIdParameterName))
            {
                userIdString = request.Query[_userIdParameterName];
            }

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var requestedUserId))
            {
                context.Result = new BadRequestObjectResult("Invalid or missing user ID");
                return;
            }

            // Allow admin override if enabled
            if (_allowAdminOverride)
            {
                var userRoles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                if (userRoles.Contains("Admin"))
                {
                    return;
                }
            }

            // Check if the requested user ID matches the current user
            var currentUserIdClaim = user.FindFirst("UserId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim.Value, out var currentUserId))
            {
                if (currentUserId == requestedUserId)
                {
                    return; // User is accessing their own data
                }
            }

            // User doesn't have access to this user's data
            context.Result = new ForbidResult();
        }
    }
}