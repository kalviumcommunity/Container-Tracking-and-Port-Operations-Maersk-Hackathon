using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/user-management")]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Get users with pagination and filtering
        /// </summary>
        [HttpGet("users")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<UsersPagedResponse>), 200)]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterDto filter)
        {
            var users = await _userManagementService.GetUsersAsync(filter);
            return Ok(ApiResponse<UsersPagedResponse>.Ok(users));
        }

        /// <summary>
        /// Update user roles
        /// </summary>
        [HttpPut("users/{userId}/roles")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateUserRoles(int userId, [FromBody] UpdateUserRolesDto updateDto)
        {
            try
            {
                await _userManagementService.UpdateUserRolesAsync(userId, updateDto);
                return Ok(ApiResponse<object>.OkWithMessage("User roles updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Update user status (block/unblock)
        /// </summary>
        [HttpPut("users/{userId}/status")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> UpdateUserStatus(int userId, [FromBody] UpdateUserStatusDto updateDto)
        {
            try
            {
                await _userManagementService.UpdateUserStatusAsync(userId, updateDto);
                return Ok(ApiResponse<object>.OkWithMessage("User status updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        [HttpGet("statistics")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<SystemStatsDto>), 200)]
        public async Task<IActionResult> GetSystemStatistics()
        {
            var stats = await _userManagementService.GetSystemStatsAsync();
            return Ok(ApiResponse<SystemStatsDto>.Ok(stats));
        }

        /// <summary>
        /// Delete user
        /// </summary>
        [HttpDelete("users/{userId}")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteUser(int userId, [FromQuery] string reason = "")
        {
            try
            {
                await _userManagementService.DeleteUserAsync(userId, reason);
                return Ok(ApiResponse<object>.OkWithMessage("User deleted successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
    }
}