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
    public class BerthsController : ControllerBase
    {
        private readonly IBerthService _berthService;

        public BerthsController(IBerthService berthService)
        {
            _berthService = berthService;
        }

        /// <summary>
        /// Gets all berths
        /// </summary>
        /// <returns>All berths</returns>
        [HttpGet]
        [RequirePermission("ViewBerths")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthDto>>), 200)]
        public async Task<IActionResult> GetAllBerths()
        {
            var berths = await _berthService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<BerthDto>>.Ok(berths));
        }

        /// <summary>
        /// Gets a berth by ID
        /// </summary>
        /// <param name="id">The ID of the berth</param>
        /// <returns>The berth</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewBerths")]
        [ProducesResponseType(typeof(ApiResponse<BerthDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetBerth(int id)
        {
            var berth = await _berthService.GetByIdAsync(id);
            if (berth == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Berth with ID {id} not found"));
            }
            
            return Ok(ApiResponse<BerthDto>.Ok(berth));
        }
        
        /// <summary>
        /// Gets detailed information about a berth
        /// </summary>
        /// <param name="id">The ID of the berth</param>
        /// <returns>The berth with detailed information</returns>
        [HttpGet("{id}/details")]
        [RequirePermission("ViewBerths")]
        [ProducesResponseType(typeof(ApiResponse<BerthDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetBerthDetails(int id)
        {
            var berth = await _berthService.GetBerthDetailAsync(id);
            if (berth == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Berth with ID {id} not found"));
            }
            
            return Ok(ApiResponse<BerthDetailDto>.Ok(berth));
        }
        
        /// <summary>
        /// Gets berths by port
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Berths at the specified port</returns>
        [HttpGet("port/{portId}")]
        [RequirePermission("ViewBerths")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthDto>>), 200)]
        public async Task<IActionResult> GetBerthsByPort(int portId)
        {
            var berths = await _berthService.GetByPortAsync(portId);
            return Ok(ApiResponse<IEnumerable<BerthDto>>.Ok(berths));
        }
        
        /// <summary>
        /// Gets berths by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Berths with the specified status</returns>
        [HttpGet("status/{status}")]
        [RequirePermission("ViewBerths")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BerthDto>>), 200)]
        public async Task<IActionResult> GetBerthsByStatus(string status)
        {
            var berths = await _berthService.GetByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<BerthDto>>.Ok(berths));
        }
        
        /// <summary>
        /// Creates a new berth
        /// </summary>
        /// <param name="createDto">Berth data</param>
        /// <returns>The created berth</returns>
        [HttpPost]
        [RequirePermission("ManageBerths")]
        [ProducesResponseType(typeof(ApiResponse<BerthDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateBerth([FromBody] BerthCreateUpdateDto createDto)
        {
            try
            {
                var berth = await _berthService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetBerth), new { id = berth.BerthId }, 
                    ApiResponse<BerthDto>.Ok(berth));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Updates a berth
        /// </summary>
        /// <param name="id">The ID of the berth to update</param>
        /// <param name="updateDto">Updated berth data</param>
        /// <returns>The updated berth</returns>
        [HttpPut("{id}")]
        [RequirePermission("ManageBerths")]
        [ProducesResponseType(typeof(ApiResponse<BerthDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateBerth(int id, [FromBody] BerthCreateUpdateDto updateDto)
        {
            try
            {
                var berth = await _berthService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<BerthDto>.Ok(berth));
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
        /// Deletes a berth
        /// </summary>
        /// <param name="id">The ID of the berth to delete</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [RequirePermission("ManageBerths")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteBerth(int id)
        {
            var result = await _berthService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Berth with ID {id} not found"));
            }
            
            return Ok(ApiResponse<object>.OkWithMessage($"Berth with ID {id} deleted successfully"));
        }
    }
}