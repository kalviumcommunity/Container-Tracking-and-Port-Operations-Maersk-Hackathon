using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    /// <summary>
    /// Controller for user management operations (Admin only)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementService userManagementService,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of users (Admin only)
        /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsersPagedResponse>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] bool includeDeleted = false)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var result = await _userManagementService.GetUsersAsync(page, pageSize, search, includeDeleted);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users list");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        /// <summary>
        /// Get user details by ID (Admin only)
        /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserListDto>> GetUser(int id)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user" });
            }
        }

        /// <summary>
        /// Update user information (Admin only)
        /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = await _userManagementService.UserExistsAsync(id);
                if (!userExists)
                {
                    return NotFound(new { message = "User not found" });
                }

                var success = await _userManagementService.UpdateUserAsync(id, updateDto);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update user. Email might already be in use." });
                }

                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the user" });
            }
        }

        /// <summary>
        /// Update user roles (Admin only)
        /// </summary>
    [HttpPut("{id}/roles")]
    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoles(int id, [FromBody] UpdateUserRolesDto rolesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = await _userManagementService.UserExistsAsync(id);
                if (!userExists)
                {
                    return NotFound(new { message = "User not found" });
                }

                var success = await _userManagementService.UpdateUserRolesAsync(id, rolesDto);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update user roles. Some roles might not exist." });
                }

                return Ok(new { message = "User roles updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roles for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating user roles" });
            }
        }

        /// <summary>
        /// Block or unblock a user (Admin only)
        /// </summary>
    [HttpPost("{id}/block")]
    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserBlock(int id, [FromBody] BlockUserDto blockDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = await _userManagementService.UserExistsAsync(id);
                if (!userExists)
                {
                    return NotFound(new { message = "User not found" });
                }

                var success = await _userManagementService.BlockUserAsync(id, blockDto.IsBlocked);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update user block status" });
                }

                var action = blockDto.IsBlocked ? "blocked" : "unblocked";
                var logMessage = !string.IsNullOrWhiteSpace(blockDto.Reason) 
                    ? $"User {action} successfully. Reason: {blockDto.Reason}" 
                    : $"User {action} successfully";
                    
                _logger.LogInformation("User {UserId} {Action}. Reason: {Reason}", 
                    id, action, blockDto.Reason ?? "No reason provided");
                    
                return Ok(new { message = logMessage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating block status for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating user block status" });
            }
        }

        /// <summary>
        /// Soft delete or restore a user (Admin only)
        /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserDelete(int id, [FromQuery] bool restore = false)
        {
            try
            {
                var success = await _userManagementService.DeleteUserAsync(id, !restore);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update user delete status" });
                }

                var action = restore ? "restored" : "deleted";
                return Ok(new { message = $"User {action} successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delete status for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating user delete status" });
            }
        }

        /// <summary>
        /// Update user status (Admin only)
        /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = await _userManagementService.UserExistsAsync(id);
                if (!userExists)
                {
                    return NotFound(new { message = "User not found" });
                }

                var success = await _userManagementService.UpdateUserStatusAsync(id, statusDto);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update user status" });
                }

                return Ok(new { message = "User status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating user status" });
            }
        }

        /// <summary>
        /// Get system statistics (Admin only)
        /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SystemStatsDto>> GetSystemStats()
        {
            try
            {
                var stats = await _userManagementService.GetSystemStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving system statistics" });
            }
        }

        /// <summary>
        /// Get available roles in the system (Admin only)
        /// </summary>
    [HttpGet("roles")]
    [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetAvailableRoles()
        {
            try
            {
                var roles = await _userManagementService.GetAvailableRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available roles");
                return StatusCode(500, new { message = "An error occurred while retrieving available roles" });
            }
        }
    }
}