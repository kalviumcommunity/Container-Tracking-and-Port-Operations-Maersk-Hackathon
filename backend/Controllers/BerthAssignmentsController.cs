using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BerthAssignmentsController : ControllerBase
    {
        private readonly IBerthAssignmentService _berthAssignmentService;

        public BerthAssignmentsController(IBerthAssignmentService berthAssignmentService)
        {
            _berthAssignmentService = berthAssignmentService;
        }

        /// <summary>
        /// Gets all berth assignments
        /// </summary>
        /// <returns>All berth assignments</returns>
        [HttpGet]
        [RequirePermission("ViewBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthAssignmentDto>>), 200)]
        public async Task<IActionResult> GetAllBerthAssignments()
        {
            var assignments = await _berthAssignmentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<BerthAssignmentDto>>.Ok(assignments));
        }

        /// <summary>
        /// Gets a berth assignment by ID
        /// </summary>
        /// <param name="id">The ID of the berth assignment</param>
        /// <returns>The berth assignment</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<BerthAssignmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetBerthAssignment(int id)
        {
            var assignment = await _berthAssignmentService.GetByIdAsync(id);
            if (assignment == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Berth assignment with ID {id} not found"));
            }
            
            return Ok(ApiResponse<BerthAssignmentDto>.Ok(assignment));
        }
        
        /// <summary>
        /// Gets assignments by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Assignments for the specified container</returns>
        [HttpGet("container/{containerId}")]
        [RequirePermission("ViewBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthAssignmentDto>>), 200)] /// 
        public async Task<IActionResult> GetAssignmentsByContainer(string containerId)
        {
            var assignments = await _berthAssignmentService.GetByContainerIdAsync(containerId);
            return Ok(ApiResponse<IEnumerable<BerthAssignmentDto>>.Ok(assignments));
        }
        
        /// <summary>
        /// Gets assignments by berth ID
        /// </summary>
        /// <param name="berthId">The berth ID</param>
        /// <returns>Assignments for the specified berth</returns>
        [HttpGet("berth/{berthId}")]
        [RequirePermission("ViewBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthAssignmentDto>>), 200)]
        public async Task<IActionResult> GetAssignmentsByBerth(int berthId)
        {
            var assignments = await _berthAssignmentService.GetByBerthIdAsync(berthId);
            return Ok(ApiResponse<IEnumerable<BerthAssignmentDto>>.Ok(assignments));
        }
        
        /// <summary>
        /// Gets active assignments (where ReleasedAt is null)
        /// </summary>
        /// <returns>All active assignments</returns>
        [HttpGet("active")]
        [RequirePermission("ViewBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthAssignmentDto>>), 200)]
        public async Task<IActionResult> GetActiveAssignments()
        {
            var assignments = await _berthAssignmentService.GetActiveAssignmentsAsync();
            return Ok(ApiResponse<IEnumerable<BerthAssignmentDto>>.Ok(assignments));
        }
        
        /// <summary>
        /// Gets assignments within a date range
        /// </summary>
        /// <param name="startDate">The start date (yyyy-MM-dd format)</param>
        /// <param name="endDate">The end date (yyyy-MM-dd format)</param>
        /// <returns>Assignments within the specified date range</returns>
        [HttpGet("daterange")]
        [RequirePermission("ViewBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthAssignmentDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetAssignmentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var assignments = await _berthAssignmentService.GetByDateRangeAsync(startDate, endDate);
                return Ok(ApiResponse<IEnumerable<BerthAssignmentDto>>.Ok(assignments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Creates a new berth assignment
        /// </summary>
        /// <param name="createDto">Berth assignment data</param>
        /// <returns>The created berth assignment</returns>
        [HttpPost]
        [RequirePermission("ManageBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<BerthAssignmentDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateBerthAssignment([FromBody] BerthAssignmentCreateUpdateDto createDto)
        {
            try
            {
                var assignment = await _berthAssignmentService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetBerthAssignment), new { id = assignment.Id }, 
                    ApiResponse<BerthAssignmentDto>.Ok(assignment));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Updates a berth assignment
        /// </summary>
        /// <param name="id">The ID of the berth assignment to update</param>
        /// <param name="updateDto">Updated berth assignment data</param>
        /// <returns>The updated berth assignment</returns>
        [HttpPut("{id}")]
        [RequirePermission("ManageBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<BerthAssignmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateBerthAssignment(int id, [FromBody] BerthAssignmentCreateUpdateDto updateDto)
        {
            try
            {
                var assignment = await _berthAssignmentService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<BerthAssignmentDto>.Ok(assignment));
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
        /// Releases a container from a berth
        /// </summary>
        /// <param name="id">The ID of the assignment to release</param>
        /// <returns>The updated assignment</returns>
        [HttpPut("{id}/release")]
        [RequirePermission("ManageBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<BerthAssignmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ReleaseContainer(int id)
        {
            try
            {
                var assignment = await _berthAssignmentService.ReleaseContainerAsync(id);
                return Ok(ApiResponse<BerthAssignmentDto>.Ok(assignment));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Deletes a berth assignment
        /// </summary>
        /// <param name="id">The ID of the berth assignment to delete</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [RequirePermission("ManageBerthAssignments")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteBerthAssignment(int id)
        {
            var result = await _berthAssignmentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Berth assignment with ID {id} not found"));
            }
            
            return Ok(ApiResponse<object>.OkWithMessage($"Berth assignment with ID {id} deleted successfully"));
        }
    }
}