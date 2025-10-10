using Backend.DTOs;
using Backend.Models;
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
    public class ContainersController : ControllerBase
    {
        private readonly IContainerService _containerService;

        public ContainersController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        /// <summary>
        /// Gets containers with pagination and filtering
        /// </summary>
        /// <param name="filter">Filter and pagination parameters</param>
        /// <returns>Paginated containers</returns>
        [HttpGet]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<ContainerDto>>), 200)]
        public async Task<IActionResult> GetContainers([FromQuery] ContainerFilterDto filter)
        {
            var containers = await _containerService.GetFilteredContainersAsync(filter);
            return Ok(ApiResponse<PaginatedResponse<ContainerDto>>.Ok(containers));
        }

        /// <summary>
        /// Gets all containers (legacy endpoint - use GET / with pagination instead)
        /// </summary>
        /// <returns>All containers</returns>
        [HttpGet("all")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContainerDto>>), 200)]
        public async Task<IActionResult> GetAllContainers()
        {
            var containers = await _containerService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<ContainerDto>>.Ok(containers));
        }

        /// <summary>
        /// Gets a container by ID
        /// </summary>
        /// <param name="id">The ID of the container</param>
        /// <returns>The container</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetContainer(string id)
        {
            var container = await _containerService.GetByIdAsync(id);
            if (container == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Container with ID {id} not found"));
            }
            
            return Ok(ApiResponse<ContainerDto>.Ok(container));
        }
        
        /// <summary>
        /// Gets detailed information about a container
        /// </summary>
        /// <param name="id">The ID of the container</param>
        /// <returns>The container with detailed information</returns>
        [HttpGet("{id}/details")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetContainerDetails(string id)
        {
            var container = await _containerService.GetContainerDetailAsync(id);
            if (container == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Container with ID {id} not found"));
            }
            
            return Ok(ApiResponse<ContainerDetailDto>.Ok(container));
        }
        
        /// <summary>
        /// Gets containers by location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Containers at the specified location</returns>
        [HttpGet("location/{location}")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContainerDto>>), 200)]
        public async Task<IActionResult> GetContainersByLocation(string location)
        {
            var containers = await _containerService.GetByLocationAsync(location);
            return Ok(ApiResponse<IEnumerable<ContainerDto>>.Ok(containers));
        }
        
        /// <summary>
        /// Gets containers by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Containers with the specified status</returns>
        [HttpGet("status/{status}")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContainerDto>>), 200)]
        public async Task<IActionResult> GetContainersByStatus(string status)
        {
            var containers = await _containerService.GetByStatusAsync(status);
            return Ok(ApiResponse<IEnumerable<ContainerDto>>.Ok(containers));
        }
        
        /// <summary>
        /// Gets containers by ship
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Containers on the specified ship</returns>
        [HttpGet("ship/{shipId}")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContainerDto>>), 200)]
        public async Task<IActionResult> GetContainersByShipId(int shipId)
        {
            var containers = await _containerService.GetByShipIdAsync(shipId);
            return Ok(ApiResponse<IEnumerable<ContainerDto>>.Ok(containers));
        }
        
        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="createDto">Container data</param>
        /// <returns>The created container</returns>
        [HttpPost]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateContainer([FromBody] ContainerCreateDto createDto)
        {
            try
            {
                // Convert to the enhanced legacy DTO with all fields
                var legacyDto = new ContainerCreateUpdateDto
                {
                    ContainerId = createDto.ContainerId,
                    CargoType = createDto.CargoType,
                    CargoDescription = createDto.CargoDescription,
                    Type = createDto.Type,
                    Status = createDto.Status,
                    Condition = createDto.Condition,
                    CurrentLocation = createDto.CurrentLocation,
                    Destination = createDto.Destination,
                    Weight = createDto.Weight,
                    MaxWeight = createDto.MaxWeight,
                    Size = createDto.Size,
                    Temperature = createDto.Temperature,
                    Coordinates = createDto.Coordinates,
                    ShipId = createDto.ShipId
                };
                
                var container = await _containerService.CreateAsync(legacyDto);
                return CreatedAtAction(nameof(GetContainer), new { id = container.ContainerId }, 
                    ApiResponse<ContainerDto>.Ok(container));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }
        
        /// <summary>
        /// Updates a container
        /// </summary>
        /// <param name="id">The ID of the container to update</param>
        /// <param name="updateDto">Updated container data</param>
        /// <returns>The updated container</returns>
        [HttpPut("{id}")]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateContainer(string id, [FromBody] ContainerUpdateDto updateDto)
        {
            try
            {
                // Convert to the enhanced legacy DTO with all fields
                var legacyDto = new ContainerCreateUpdateDto
                {
                    ContainerId = id, // Use the ID from the URL
                    CargoType = updateDto.CargoType,
                    CargoDescription = updateDto.CargoDescription,
                    Type = updateDto.Type,
                    Status = updateDto.Status,
                    Condition = updateDto.Condition,
                    CurrentLocation = updateDto.CurrentLocation,
                    Destination = updateDto.Destination,
                    Weight = updateDto.Weight ?? 0,
                    MaxWeight = updateDto.MaxWeight,
                    Size = updateDto.Size,
                    Temperature = updateDto.Temperature,
                    Coordinates = updateDto.Coordinates,
                    ShipId = updateDto.ShipId
                };
                
                var container = await _containerService.UpdateAsync(id, legacyDto);
                return Ok(ApiResponse<ContainerDto>.Ok(container));
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
        /// Gets container statistics and analytics
        /// </summary>
        /// <returns>Container statistics</returns>
        [HttpGet("statistics")]
        [RequirePermission("ViewContainers")]
        [ProducesResponseType(typeof(ApiResponse<ContainerStatsDto>), 200)]
        public async Task<IActionResult> GetContainerStatistics()
        {
            var stats = await _containerService.GetContainerStatisticsAsync();
            return Ok(ApiResponse<ContainerStatsDto>.Ok(stats));
        }

        /// <summary>
        /// Bulk update container statuses
        /// </summary>
        /// <param name="bulkUpdate">Bulk update request</param>
        /// <returns>Update results</returns>
        [HttpPatch("bulk-status")]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<BulkUpdateResultDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkStatusUpdateDto bulkUpdate)
        {
            try
            {
                var result = await _containerService.BulkUpdateStatusAsync(bulkUpdate);
                return Ok(ApiResponse<BulkUpdateResultDto>.Ok(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Export containers to CSV
        /// </summary>
        /// <param name="filter">Filter parameters for export</param>
        /// <returns>CSV file</returns>
        [HttpGet("export")]
        [RequirePermission("ViewContainers")]
        public async Task<IActionResult> ExportContainers([FromQuery] ContainerFilterDto filter)
        {
            var csvData = await _containerService.ExportContainersAsync(filter);
            return File(csvData, "text/csv", $"containers_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
        
        /// <summary>
        /// Deletes a container
        /// </summary>
        /// <param name="id">The ID of the container to delete</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> DeleteContainer(string id)
        {
            var result = await _containerService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail($"Container with ID {id} not found"));
            }
            
            return Ok(ApiResponse<object>.OkWithMessage($"Container with ID {id} deleted successfully"));
        }
    }
}