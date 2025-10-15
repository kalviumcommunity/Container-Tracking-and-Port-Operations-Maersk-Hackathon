using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/ship-routes")]
    [Authorize]
    public class ShipRoutesController : ControllerBase
    {
        private readonly IShipRouteService _shipRouteService;

        public ShipRoutesController(IShipRouteService shipRouteService)
        {
            _shipRouteService = shipRouteService;
        }

        [HttpGet]
        [RequirePermission("ViewShips")]
        public async Task<IActionResult> GetAllShipRoutes()
        {
            var routes = await _shipRouteService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(routes));
        }

        [HttpGet("{id}")]
        [RequirePermission("ViewShips")]
        public async Task<IActionResult> GetShipRoute(int id)
        {
            var route = await _shipRouteService.GetByIdAsync(id);
            if (route == null)
            {
                return NotFound(ApiResponse<object>.Fail("Ship route not found"));
            }
            return Ok(ApiResponse<object>.Ok(route));
        }

        [HttpGet("ship/{shipId}")]
        [RequirePermission("ViewShips")]
        public async Task<IActionResult> GetRoutesForShip(int shipId)
        {
            var routes = await _shipRouteService.GetRoutesForShipAsync(shipId);
            return Ok(ApiResponse<object>.Ok(routes));
        }

        [HttpPost]
        [RequirePermission("ScheduleShips")]
        public async Task<IActionResult> CreateShipRoute([FromBody] ShipRouteCreateUpdateDto createDto)
        {
            var route = await _shipRouteService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetShipRoute), new { id = route.Id }, ApiResponse<object>.Ok(route));
        }

        [HttpPut("{id}")]
        [RequirePermission("ScheduleShips")]
        public async Task<IActionResult> UpdateShipRoute(int id, [FromBody] ShipRouteCreateUpdateDto updateDto)
        {
            try
            {
                var route = await _shipRouteService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<object>.Ok(route));
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Ship route not found"));
            }
        }

        [HttpDelete("{id}")]
        [RequirePermission("ScheduleShips")]
        public async Task<IActionResult> DeleteShipRoute(int id)
        {
            var result = await _shipRouteService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail("Ship route not found"));
            }
            return Ok(ApiResponse<object>.OkWithMessage("Ship route deleted successfully"));
        }
    }
}
