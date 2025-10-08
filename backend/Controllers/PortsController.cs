using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Backend.Constants;
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
    public class PortsController : ControllerBase
    {
        private readonly IPortService _portService;

        public PortsController(IPortService portService)
        {
            _portService = portService;
        }

        /// <summary>
        /// Gets all ports
        /// </summary>
        /// <returns>All ports</returns>
        [HttpGet]
        [RequirePermission("ViewPortDetails")] // Changed to match available permission
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PortDto>>), 200)]
        public async Task<IActionResult> GetAllPorts()
        {
            var ports = await _portService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<PortDto>>.Ok(ports));
        }

        /// <summary>
        /// Gets a port by ID
        /// </summary>
        /// <param name="id">The ID of the port</param>
        /// <returns>The port</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewPortDetails")]
        [ProducesResponseType(typeof(ApiResponse<PortDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetPort(int id)
        {
            var port = await _portService.GetByIdAsync(id);
            if (port == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Port with ID {id} not found"));
            }
            
            return Ok(ApiResponse<PortDto>.Ok(port));
        }
        
        /// <summary>
        /// Gets detailed information about a port
        /// </summary>
        /// <param name="id">The ID of the port</param>
        /// <returns>The port with detailed information</returns>
        [HttpGet("{id}/details")]
        [RequirePermission("ViewPortDetails")]
        [ProducesResponseType(typeof(ApiResponse<PortDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetPortDetails(int id)
        {
            var port = await _portService.GetPortDetailAsync(id);
            if (port == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Port with ID {id} not found"));
            }
            
            return Ok(ApiResponse<PortDetailDto>.Ok(port));
        }
        
        /// <summary>
        /// Gets ports by location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Ports at the specified location</returns>
        [HttpGet("location/{location}")]
        [RequirePermission("ViewPortDetails")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PortDto>>), 200)]
        public async Task<IActionResult> GetPortsByLocation(string location)
        {
            var ports = await _portService.GetByLocationAsync(location);
            return Ok(ApiResponse<IEnumerable<PortDto>>.Ok(ports));
        }
        
        /// <summary>
        /// Creates a new port
        /// </summary>
        /// <param name="createDto">Port data</param>
        /// <returns>The created port</returns>
        [HttpPost]
        [RequirePermission("ManageAllPorts")] // Changed to match available permission
        [ProducesResponseType(typeof(ApiResponse<PortDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreatePort([FromBody] PortCreateUpdateDto createDto)
        {
            try
            {
                var port = await _portService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetPort), new { id = port.PortId }, 
                    ApiResponse<PortDto>.Ok(port));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Updates a port
        /// </summary>
        /// <param name="id">The ID of the port to update</param>
        /// <param name="updateDto">Updated port data</param>
        /// <returns>The updated port</returns>
    [HttpPut("{id}")]
    [RequirePermission("ManagePortDetails")]
        [ProducesResponseType(typeof(ApiResponse<PortDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdatePort(int id, [FromBody] PortCreateUpdateDto updateDto)
        {
            try
            {
                var port = await _portService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<PortDto>.Ok(port));
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
        /// Deletes a port
        /// </summary>
        /// <param name="id">The ID of the port to delete</param>
        /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [RequirePermission("ManageAllPorts")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeletePort(int id)
        {
            var result = await _portService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Port with ID {id} not found"));
            }
            
            return Ok(ApiResponse<object>.OkWithMessage($"Port with ID {id} deleted successfully"));
        }
    }
}