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
    public class ShipsController : ControllerBase
    {
        private readonly IShipService _shipService;

        public ShipsController(IShipService shipService)
        {
            _shipService = shipService;
        }

        /// <summary>
        /// Gets all ships
        /// </summary>
        /// <returns>All ships</returns>
        [HttpGet]
        [RequirePermission("ViewShips")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShipDto>>), 200)]
        public async Task<IActionResult> GetAllShips()
        {
            var ships = await _shipService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ShipDto>>.Ok(ships));
        }

        /// <summary>
        /// Gets a ship by ID
        /// </summary>
        /// <param name="id">The ID of the ship</param>
        /// <returns>The ship</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewShips")]
        [ProducesResponseType(typeof(ApiResponse<ShipDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetShip(int id)
        {
            var ship = await _shipService.GetByIdAsync(id);
            if (ship == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Ship with ID {id} not found"));
            }
            
            return Ok(ApiResponse<ShipDto>.Ok(ship));
        }
        
        /// <summary>
        /// Gets detailed information about a ship
        /// </summary>
        /// <param name="id">The ID of the ship</param>
        /// <returns>The ship with detailed information</returns>
        [HttpGet("{id}/details")]
        [RequirePermission("ViewShips")]
        [ProducesResponseType(typeof(ApiResponse<ShipDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetShipDetails(int id)
        {
            var ship = await _shipService.GetShipDetailAsync(id);
            if (ship == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Ship with ID {id} not found"));
            }
            
            return Ok(ApiResponse<ShipDetailDto>.Ok(ship));
        }
        
        /// <summary>
        /// Gets ships by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Ships with the specified status</returns>
        [HttpGet("status/{status}")]
        [RequirePermission("ViewShips")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShipDto>>), 200)]
        public async Task<IActionResult> GetShipsByStatus(string status)
        {
            var ships = await _shipService.GetByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<ShipDto>>.Ok(ships));
        }
        
        /// <summary>
        /// Creates a new ship
        /// </summary>
        /// <param name="createDto">Ship data</param>
        /// <returns>The created ship</returns>
        [HttpPost]
        [RequirePermission("ManageShips")]
        [ProducesResponseType(typeof(ApiResponse<ShipDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateShip([FromBody] ShipCreateUpdateDto createDto)
        {
            try
            {
                var ship = await _shipService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetShip), new { id = ship.ShipId }, 
                    ApiResponse<ShipDto>.Ok(ship));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Updates a ship
        /// </summary>
        /// <param name="id">The ID of the ship to update</param>
        /// <param name="updateDto">Updated ship data</param>
        /// <returns>The updated ship</returns>
        [HttpPut("{id}")]
        [RequirePermission("ManageShips")]
        [ProducesResponseType(typeof(ApiResponse<ShipDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateShip(int id, [FromBody] ShipCreateUpdateDto updateDto)
        {
            try
            {
                var ship = await _shipService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<ShipDto>.Ok(ship));
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
        /// Deletes a ship
        /// </summary>
        /// <param name="id">The ID of the ship to delete</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [RequirePermission("ManageShips")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteShip(int id)
        {
            var result = await _shipService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Ship with ID {id} not found"));
            }
            
            return Ok(ApiResponse<object>.OkWithMessage($"Ship with ID {id} deleted successfully"));
        }
    }
}