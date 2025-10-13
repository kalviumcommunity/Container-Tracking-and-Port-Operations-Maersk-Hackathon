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
        private readonly ILogger<RoleApplicationController> _logger;

        public RoleApplicationController(
            IRoleApplicationService roleApplicationService,
            ILogger<RoleApplicationController> logger)
        {
            _roleApplicationService = roleApplicationService;
            _logger = logger;
        }

        /// <summary>
        /// Submit a new role application
        /// </summary>
        /// <param name="request">Role application request</param>
        /// <returns>Created application</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleApplicationDto>), 201)]
        public async Task<IActionResult> SubmitApplication([FromBody] RoleApplicationRequestDto requestDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var application = await _roleApplicationService.SubmitApplicationAsync(userId, requestDto);
                
                return CreatedAtAction(nameof(GetApplication), 
                    new { id = application.ApplicationId }, 
                    ApiResponse<RoleApplicationDto>.Ok(application));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting role application");
                return BadRequest(ApiResponse<object>.Fail("Error submitting application"));
            }
        }

        /// <summary>
        /// Get user's own role applications
        /// </summary>
        /// <returns>List of user's applications</returns>
        [HttpGet("my-applications")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleApplicationDto>>), 200)]
        public async Task<IActionResult> GetMyApplications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var applications = await _roleApplicationService.GetUserApplicationsAsync(userId);
                return Ok(ApiResponse<IEnumerable<RoleApplicationDto>>.Ok(applications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user applications");
                return BadRequest(ApiResponse<object>.Fail("Error retrieving applications"));
            }
        }

        /// <summary>
        /// Get all pending role applications (Admin only)
        /// </summary>
        /// <returns>List of pending applications</returns>
        [HttpGet("pending")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleApplicationDto>>), 200)]
        public async Task<IActionResult> GetPendingApplications()
        {
            try
            {
                var applications = await _roleApplicationService.GetPendingApplicationsAsync();
                return Ok(ApiResponse<IEnumerable<RoleApplicationDto>>.Ok(applications));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending applications");
                return BadRequest(ApiResponse<object>.Fail("Error retrieving applications"));
            }
        }

        /// <summary>
        /// Get application by ID
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <returns>Role application details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleApplicationDto>), 200)]
        public async Task<IActionResult> GetApplication(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var application = await _roleApplicationService.GetApplicationAsync(id);
                
                if (application == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Application not found"));
                }

                // Users can only see their own applications unless they're admin
                if (application.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                return Ok(ApiResponse<RoleApplicationDto>.Ok(application));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application {ApplicationId}", id);
                return BadRequest(ApiResponse<object>.Fail("Error retrieving application"));
            }
        }

        /// <summary>
        /// Review a role application (Admin only)
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="review">Review decision and notes</param>
        /// <returns>Success result</returns>
        [HttpPut("{id}/review")]
        [RequirePermission("ManageUsers")]
        [ProducesResponseType(typeof(ApiResponse<RoleApplicationDto>), 200)]
        public async Task<IActionResult> ReviewApplication(int id, [FromBody] ReviewApplicationDto reviewDto)
        {
            try
            {
                var reviewerId = GetCurrentUserId();
                var success = await _roleApplicationService.ReviewApplicationAsync(id, reviewerId, reviewDto);
                
                if (!success)
                {
                    return NotFound(ApiResponse<object>.Fail("Application not found"));
                }

                var application = await _roleApplicationService.GetApplicationAsync(id);
                return Ok(ApiResponse<RoleApplicationDto>.Ok(application));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reviewing application {ApplicationId}", id);
                return BadRequest(ApiResponse<object>.Fail("Error reviewing application"));
            }
        }

        /// <summary>
        /// Get available roles for application
        /// </summary>
        /// <returns>List of available roles</returns>
        [HttpGet("available-roles")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AvailableRoleDto>>), 200)]
        public async Task<IActionResult> GetAvailableRoles()
        {
            try
            {
                var userId = GetCurrentUserId();
                var roles = await _roleApplicationService.GetAvailableRolesAsync(userId);
                return Ok(ApiResponse<IEnumerable<AvailableRoleDto>>.Ok(roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available roles");
                return BadRequest(ApiResponse<object>.Fail("Error retrieving roles"));
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