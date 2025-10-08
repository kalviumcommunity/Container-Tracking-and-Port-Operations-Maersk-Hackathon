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
    public class ShipContainersController : ControllerBase
    {
        private readonly IShipContainerService _shipContainerService;

        public ShipContainersController(IShipContainerService shipContainerService)
        {
            _shipContainerService = shipContainerService;
        }

        /// <summary>
        /// Gets all ship container operations
        /// </summary>
        /// <returns>All ship container operations</returns>
        [HttpGet]
        [RequirePermission("ViewCargo")] // Changed from ViewShipContainers to ViewCargo
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShipContainerDto>>), 200)]
        public async Task<IActionResult> GetAllShipContainers()
        {
            var operations = await _shipContainerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ShipContainerDto>>.Ok(operations));
        }

        /// <summary>
        /// Gets a ship container operation by ID
        /// </summary>
        /// <param name="id">The ID of the ship container operation</param>
        /// <returns>The ship container operation</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewCargo")] // Changed from ViewShipContainers to ViewCargo
        [ProducesResponseType(typeof(ApiResponse<ShipContainerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetShipContainer(int id)
        {
            var operation = await _shipContainerService.GetByIdAsync(id);
            if (operation == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Ship container operation with ID {id} not found"));
            }
            
            return Ok(ApiResponse<ShipContainerDto>.Ok(operation));
        }
        
        /// <summary>
        /// Gets operations by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Operations for the specified container</returns>
        [HttpGet("container/{containerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShipContainerDto>>), 200)]
        public async Task<IActionResult> GetOperationsByContainer(string containerId)
        {
            var operations = await _shipContainerService.GetByContainerIdAsync(containerId);
            return Ok(ApiResponse<IEnumerable<ShipContainerDto>>.Ok(operations));
        }
        
        /// <summary>
        /// Gets operations by ship ID
        /// </summary>
        /// <param name="shipId">The ship ID</param>
        /// <returns>Operations for the specified ship</returns>
        [HttpGet("ship/{shipId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShipContainerDto>>), 200)]
        public async Task<IActionResult> GetOperationsByShip(int shipId)
        {
            var operations = await _shipContainerService.GetByShipIdAsync(shipId);
            return Ok(ApiResponse<IEnumerable<ShipContainerDto>>.Ok(operations));
        }
        
        /// <summary>
        /// Gets operations within a date range
        /// </summary>
        /// <param name="startDate">The start date (yyyy-MM-dd format)</param>
        /// <param name="endDate">The end date (yyyy-MM-dd format)</param>
        /// <returns>Operations within the specified date range</returns>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShipContainerDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetOperationsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var operations = await _shipContainerService.GetByDateRangeAsync(startDate, endDate);
                return Ok(ApiResponse<IEnumerable<ShipContainerDto>>.Ok(operations));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Creates a new ship container operation
        /// </summary>
        /// <param name="createDto">Ship container operation data</param>
        /// <returns>The created ship container operation</returns>
        [HttpPost]
        [RequirePermission("ManageCargo")] // Added proper permission
        [ProducesResponseType(typeof(ApiResponse<ShipContainerDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateShipContainer([FromBody] ShipContainerCreateUpdateDto createDto)
        {
            try
            {
                var operation = await _shipContainerService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetShipContainer), new { id = operation.Id }, 
                    ApiResponse<ShipContainerDto>.Ok(operation));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Updates a ship container operation
        /// </summary>
        /// <param name="id">The ID of the ship container operation to update</param>
        /// <param name="updateDto">Updated ship container operation data</param>
        /// <returns>The updated ship container operation</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ShipContainerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateShipContainer(int id, [FromBody] ShipContainerCreateUpdateDto updateDto)
        {
            try
            {
                var operation = await _shipContainerService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<ShipContainerDto>.Ok(operation));
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
        /// Loads a container onto a ship
        /// </summary>
        /// <param name="loadRequest">Load container request</param>
        /// <returns>The created ship container operation</returns>
        [HttpPost("load")]
        [RequirePermission("ManageCargo")] // Added proper permission
        [ProducesResponseType(typeof(ApiResponse<ShipContainerDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> LoadContainer([FromBody] LoadContainerRequest loadRequest)
        {
            try
            {
                var operation = await _shipContainerService.LoadContainerAsync(loadRequest.ShipId, loadRequest.ContainerId);
                return CreatedAtAction(nameof(GetShipContainer), new { id = operation.Id }, 
                    ApiResponse<ShipContainerDto>.Ok(operation));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Deletes a ship container operation
        /// </summary>
        /// <param name="id">The ID of the ship container operation to delete</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteShipContainer(int id)
        {
            var result = await _shipContainerService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Ship container operation with ID {id} not found"));
            }
            
            return Ok(ApiResponse<object>.OkWithMessage($"Ship container operation with ID {id} deleted successfully"));
        }
    }
    
    /// <summary>
    /// Request model for loading a container onto a ship
    /// </summary>
    public class LoadContainerRequest
    {
        /// <summary>
        /// The ID of the ship
        /// </summary>
        public int ShipId { get; set; }
        
        /// <summary>
        /// The ID of the container
        /// </summary>
        public string ContainerId { get; set; }
    }
}