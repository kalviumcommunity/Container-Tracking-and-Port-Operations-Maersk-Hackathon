using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/container-movements")]
    [Authorize]
    public class ContainerMovementsController : ControllerBase
    {
        private readonly IContainerMovementService _containerMovementService;

        public ContainerMovementsController(IContainerMovementService containerMovementService)
        {
            _containerMovementService = containerMovementService;
        }

        /// <summary>
        /// Get all movements for a specific container
        /// </summary>
        /// <param name="containerId">Container ID</param>
        /// <returns>List of container movements</returns>
        [HttpGet("container/{containerId}")]
        [RequirePermission("TrackContainers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContainerMovementDto>>), 200)]
        public async Task<IActionResult> GetContainerMovements(string containerId)
        {
            var movements = await _containerMovementService.GetByContainerIdAsync(containerId);
            return Ok(ApiResponse<IEnumerable<ContainerMovementDto>>.Ok(movements));
        }

        /// <summary>
        /// Record a new container movement
        /// </summary>
        /// <param name="createDto">Movement data</param>
        /// <returns>Created movement</returns>
        [HttpPost]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerMovementDto>), 201)]
        public async Task<IActionResult> CreateMovement([FromBody] ContainerMovementCreateDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var movement = await _containerMovementService.CreateAsync(createDto, userId);
                return CreatedAtAction(nameof(GetMovement), new { id = movement.Id }, 
                    ApiResponse<ContainerMovementDto>.Ok(movement));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Get container tracking route
        /// </summary>
        /// <param name="containerId">Container ID</param>
        /// <returns>Complete tracking route</returns>
        [HttpGet("tracking/{containerId}")]
        [RequirePermission("TrackContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerTrackingDto>), 200)]
        public async Task<IActionResult> GetContainerTracking(string containerId)
        {
            var tracking = await _containerMovementService.GetTrackingAsync(containerId);
            if (tracking == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Container {containerId} not found"));
            }
            return Ok(ApiResponse<ContainerTrackingDto>.Ok(tracking));
        }

        /// <summary>
        /// Get movement by ID
        /// </summary>
        /// <param name="id">Movement ID</param>
        /// <returns>Movement details</returns>
        [HttpGet("{id}")]
        [RequirePermission("TrackContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerMovementDto>), 200)]
        public async Task<IActionResult> GetMovement(int id)
        {
            var movement = await _containerMovementService.GetByIdAsync(id);
            if (movement == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Movement with ID {id} not found"));
            }
            return Ok(ApiResponse<ContainerMovementDto>.Ok(movement));
        }

        /// <summary>
        /// Update movement status
        /// </summary>
        /// <param name="id">Movement ID</param>
        /// <param name="updateDto">Updated movement data</param>
        /// <returns>Updated movement</returns>
        [HttpPatch("{id}")]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerMovementDto>), 200)]
        public async Task<IActionResult> UpdateMovement(int id, [FromBody] ContainerMovementUpdateDto updateDto)
        {
            try
            {
                var movement = await _containerMovementService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<ContainerMovementDto>.Ok(movement));
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
