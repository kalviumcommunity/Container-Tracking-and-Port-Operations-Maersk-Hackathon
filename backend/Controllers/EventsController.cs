using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        /// <summary>
        /// Get events with filtering
        /// </summary>
        /// <param name="filter">Filter parameters</param>
        /// <returns>Filtered events</returns>
        [HttpGet]
        [RequirePermission("ViewDashboard")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EventDto>>), 200)]
        public async Task<IActionResult> GetEvents([FromQuery] EventFilterDto filter)
        {
            var events = await _eventService.GetFilteredEventsAsync(filter);
            return Ok(ApiResponse<PaginatedResponse<EventDto>>.Ok(events));
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <param name="createDto">Event data</param>
        /// <returns>Created event</returns>
        [HttpPost]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<EventDto>), 201)]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateDto createDto)
        {
            try
            {
                var eventObj = await _eventService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetEvent), new { id = eventObj.EventId }, 
                    ApiResponse<EventDto>.Ok(eventObj));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Event details</returns>
        [HttpGet("{id}")]
        [RequirePermission("ViewDashboard")]
        [ProducesResponseType(typeof(ApiResponse<EventDto>), 200)]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventObj = await _eventService.GetByIdAsync(id);
            if (eventObj == null)
            {
                return NotFound(ApiResponse<object>.Fail($"Event with ID {id} not found"));
            }
            return Ok(ApiResponse<EventDto>.Ok(eventObj));
        }

        /// <summary>
        /// Acknowledge an event
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Updated event</returns>
        [HttpPost("{id}/acknowledge")]
        [RequirePermission("ViewDashboard")]
        [ProducesResponseType(typeof(ApiResponse<EventDto>), 200)]
        public async Task<IActionResult> AcknowledgeEvent(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var eventObj = await _eventService.AcknowledgeAsync(id, userId);
                return Ok(ApiResponse<EventDto>.Ok(eventObj));
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
        /// Resolve an event
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <param name="resolution">Resolution notes</param>
        /// <returns>Updated event</returns>
        [HttpPost("{id}/resolve")]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<EventDto>), 200)]
        public async Task<IActionResult> ResolveEvent(int id, [FromBody] EventResolutionDto resolution)
        {
            try
            {
                var userId = User.GetUserId();
                var eventObj = await _eventService.ResolveAsync(id, userId, resolution.Resolution);
                return Ok(ApiResponse<EventDto>.Ok(eventObj));
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
        /// Get user's assigned events
        /// </summary>
        /// <returns>Assigned events</returns>
        [HttpGet("my-assignments")]
        [RequirePermission("ViewDashboard")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EventDto>>), 200)]
        public async Task<IActionResult> GetMyAssignments()
        {
            var userId = User.GetUserId();
            var events = await _eventService.GetUserAssignedEventsAsync(userId);
            return Ok(ApiResponse<IEnumerable<EventDto>>.Ok(events));
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [RequirePermission("ManageContainers")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _eventService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse<object>.Fail($"Event with ID {id} not found"));
                }
                return Ok(ApiResponse<object>.OkWithMessage("Event deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", id);
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

    }

    public class EventResolutionDto
    {
        public required string Resolution { get; set; }
    }
}