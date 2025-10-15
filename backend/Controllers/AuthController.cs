using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Backend.Constants;
using System.Security.Claims;

namespace Backend.Controllers
{
    /// <summary>
    /// Authentication and user management controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginDto);
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        /// <summary>
        /// Public user registration - All users start with Viewer role for security
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // SECURITY: Force all new users to start with Viewer role only
                // Users must apply for additional roles through the role application system
                registerDto.Roles = new List<string> { "Viewer" };

                var user = await _authService.RegisterAsync(registerDto);
                
                // Auto-login after successful registration
                var loginDto = new LoginDto 
                { 
                    Username = registerDto.Username, 
                    Password = registerDto.Password 
                };
                var loginResult = await _authService.LoginAsync(loginDto);
                
                if (loginResult == null)
                {
                    return StatusCode(500, new { message = "Registration successful but auto-login failed" });
                }

                return CreatedAtAction(nameof(GetProfile), new { userId = user.UserId }, loginResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        /// <summary>
        /// User registration (Admin only)
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <returns>Created user information</returns>
        [HttpPost("admin/register")]
        [RequirePermission(Backend.Constants.Permissions.ManageUsers)]
        public async Task<ActionResult<UserDto>> AdminRegister([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _authService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(GetProfile), new { userId = user.UserId }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.GetUserId();
                var user = await _authService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    return NotFound(ApiResponse<object>.Fail("User not found"));
                }
                
                return Ok(ApiResponse<UserDto>.Ok(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return BadRequest(ApiResponse<object>.Fail("Error retrieving profile"));
            }
        }

        /// <summary>
        /// Get current user (alias for profile)
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await GetProfile();
        }

        /// <summary>
        /// Get user by ID (Admin or owner only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User information</returns>
        [HttpGet("users/{userId}")]
        [RequireOwnership("userId")]
        public async Task<ActionResult<UserDto>> GetUser(int userId)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="updateDto">Updated user information</param>
        /// <returns>Updated user information</returns>
        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var userId = User.GetUserId();
                var updatedUser = await _authService.UpdateUserAsync(userId, updateDto);
                
                if (updatedUser == null)
                {
                    return NotFound(ApiResponse<object>.Fail("User not found"));
                }
                
                return Ok(ApiResponse<UserDto>.Ok(updatedUser));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return BadRequest(ApiResponse<object>.Fail("Error updating profile"));
            }
        }

        /// <summary>
        /// Update user by ID (Admin or owner only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="updateDto">Updated user information</param>
        /// <returns>Updated user information</returns>
        [HttpPut("users/{userId}")]
        [RequireOwnership("userId")]
        public async Task<ActionResult<UserDto>> UpdateUser(int userId, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _authService.UpdateUserAsync(userId, updateDto);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Change password for current user
        /// </summary>
        /// <param name="changePasswordDto">Password change data</param>
        /// <returns>Success status</returns>
        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = User.GetUserId();
                var success = await _authService.ChangePasswordAsync(userId, changePasswordDto);
                
                if (!success)
                {
                    return BadRequest(ApiResponse<object>.Fail("Current password is incorrect"));
                }
                
                return Ok(ApiResponse<object>.OkWithMessage("Password changed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return BadRequest(ApiResponse<object>.Fail("Error changing password"));
            }
        }

        /// <summary>
        /// Assign roles to user (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleNames">Role names to assign</param>
        /// <returns>Success status</returns>
        [HttpPost("users/{userId}/roles")]
        [RequirePermission(Backend.Constants.Permissions.ManageUsers)]
        public async Task<ActionResult> AssignRoles(int userId, [FromBody] List<string> roleNames)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _authService.AssignRolesToUserAsync(userId, roleNames);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to assign roles. Check if user and roles exist." });
                }

                return Ok(new { message = "Roles assigned successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning roles", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove roles from user (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleNames">Role names to remove</param>
        /// <returns>Success status</returns>
        [HttpDelete("users/{userId}/roles")]
        [RequirePermission(Backend.Constants.Permissions.ManageUsers)]
        public async Task<ActionResult> RemoveRoles(int userId, [FromBody] List<string> roleNames)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _authService.RemoveRolesFromUserAsync(userId, roleNames);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to remove roles. Check if user exists." });
                }

                return Ok(new { message = "Roles removed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing roles", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate user account (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("users/{userId}/deactivate")]
        [RequirePermission(Backend.Constants.Permissions.ManageUsers)]
        public async Task<ActionResult> DeactivateUser(int userId)
        {
            try
            {
                var success = await _authService.DeactivateUserAsync(userId);
                if (!success)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate user account (Admin only)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("users/{userId}/activate")]
        [RequirePermission(Backend.Constants.Permissions.ManageUsers)]
        public async Task<ActionResult> ActivateUser(int userId)
        {
            try
            {
                var success = await _authService.ActivateUserAsync(userId);
                if (!success)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's roles and permissions
        /// </summary>
        /// <returns>User roles and permissions</returns>
        [HttpGet("permissions")]
        [Authorize]
        public ActionResult<object> GetCurrentUserPermissions()
        {
            try
            {
                var roles = User.GetRoles();
                var permissions = User.GetPermissions();

                return Ok(new
                {
                    roles = roles,
                    permissions = permissions,
                    userId = User.GetUserId(),
                    username = User.GetUsername(),
                    portId = User.GetPortId()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving permissions", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        /// <returns>Token validation status</returns>
        [HttpGet("validate")]
        [Authorize]
        public ActionResult ValidateToken()
        {
            return Ok(new { message = "Token is valid", userId = User.GetUserId(), username = User.GetUsername() });
        }

        /// <summary>
        /// User logout (client-side token removal)
        /// </summary>
        /// <returns>Logout confirmation</returns>
        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // JWT tokens are stateless, so logout is handled client-side by removing the token
            return Ok(new { message = "Logged out successfully. Please remove the token from client storage." });
        }


    }
}