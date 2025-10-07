using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    /// <summary>
    /// API controller for managing role applications and approvals
    /// </summary>
    [ApiController]
    [Route("api/role-applications")]
    [Authorize]
    public class RoleApplicationController : ControllerBase
    {
        private readonly IRoleApplicationService _roleApplicationService;

        public RoleApplicationController(IRoleApplicationService roleApplicationService)
        {
            _roleApplicationService = roleApplicationService;
        }

        /// <summary>
        /// Submit a new role application
        /// </summary>
        /// <param name="request">Role application request</param>
        /// <returns>Created application</returns>
        [HttpPost]
        public async Task<ActionResult<RoleApplicationDto>> SubmitApplication([FromBody] RoleApplicationRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var application = await _roleApplicationService.SubmitApplicationAsync(userId, request);
                return CreatedAtAction(nameof(GetApplication), new { id = application.ApplicationId }, application);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's role applications
        /// </summary>
        /// <returns>List of user's applications</returns>
        [HttpGet("my-applications")]
        public async Task<ActionResult<List<RoleApplicationDto>>> GetMyApplications()
        {
            var userId = GetCurrentUserId();
            var applications = await _roleApplicationService.GetUserApplicationsAsync(userId);
            return Ok(applications);
        }

        /// <summary>
        /// Get available roles that current user can apply for
        /// </summary>
        /// <returns>List of available roles</returns>
        [HttpGet("available-roles")]
        public async Task<ActionResult<List<AvailableRoleDto>>> GetAvailableRoles()
        {
            var userId = GetCurrentUserId();
            var availableRoles = await _roleApplicationService.GetAvailableRolesAsync(userId);
            return Ok(availableRoles);
        }

        /// <summary>
        /// Get a specific application by ID
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Role application details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleApplicationDto>> GetApplication(int id)
        {
            var userId = GetCurrentUserId();
            var application = await _roleApplicationService.GetApplicationAsync(id);

            if (application == null)
            {
                return NotFound(new { message = "Application not found" });
            }

            // Users can only see their own applications unless they're admin
            if (application.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Ok(application);
        }

        /// <summary>
        /// Cancel a pending application
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Success result</returns>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelApplication(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _roleApplicationService.CancelApplicationAsync(id, userId);

            if (!success)
            {
                return BadRequest(new { message = "Unable to cancel application. It may not exist or is not in pending status." });
            }

            return Ok(new { message = "Application cancelled successfully" });
        }

        /// <summary>
        /// Get all pending applications (Admin only)
        /// </summary>
        /// <returns>List of pending applications</returns>
        [HttpGet("pending")]
        [RequirePermission("ManageUsers")]
        public async Task<ActionResult<List<RoleApplicationDto>>> GetPendingApplications()
        {
            var applications = await _roleApplicationService.GetPendingApplicationsAsync();
            return Ok(applications);
        }

        /// <summary>
        /// Get all applications with any status (Admin only)
        /// </summary>
        /// <returns>List of all applications</returns>
        [HttpGet("all")]
        [RequirePermission("ManageUsers")]
        public async Task<ActionResult<List<RoleApplicationDto>>> GetAllApplications()
        {
            var applications = await _roleApplicationService.GetAllApplicationsAsync();
            return Ok(applications);
        }

        /// <summary>
        /// Review and approve/reject a role application (Admin only)
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="review">Review decision and notes</param>
        /// <returns>Success result</returns>
        [HttpPost("{id}/review")]
        [RequirePermission("ManageUsers")]
        public async Task<ActionResult> ReviewApplication(int id, [FromBody] ReviewApplicationDto review)
        {
            try
            {
                var reviewerId = GetCurrentUserId();
                var success = await _roleApplicationService.ReviewApplicationAsync(id, reviewerId, review);

                if (!success)
                {
                    return BadRequest(new { message = "Failed to review application" });
                }

                var statusMessage = review.Status == "Approved" ? "approved" : "rejected";
                return Ok(new { message = $"Application {statusMessage} successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }
    }
}