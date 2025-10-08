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
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Gets events with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter and pagination parameters</param>
        /// <returns>Paginated events</returns>
        [HttpGet]
        [RequirePermission("ViewEvents")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EventDto>>), 200)]
        public async Task<IActionResult> GetEvents([FromQuery] EventFilterDto filter)
        {
            var events = await _eventService.GetFilteredEventsAsync(filter);
            return Ok(ApiResponse<PaginatedResponse<EventDto>>.Ok(events));
        }

        /// <summary>
        /// Gets recent events for dashboard
        /// </summary>
        /// <param name="count">Number of recent events to retrieve</param>
        /// <returns>Recent events</returns>
        [HttpGet("recent")]
        [RequirePermission("ViewEvents")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EventDto>>), 200)]
        public async Task<IActionResult> GetRecentEvents([FromQuery] int count = 10)
        {
            var events = await _eventService.GetRecentEventsAsync(count);
            return Ok(ApiResponse<IEnumerable<EventDto>>.Ok(events));
        }

        /// <summary>
        /// Gets unread events for a user
        /// </summary>
        /// <returns>Unread events</returns>
        [HttpGet("unread")]
        [RequirePermission("ViewEvents")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EventDto>>), 200)]
        public async Task<IActionResult> GetUnreadEvents()
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            var events = await _eventService.GetUnreadEventsAsync(userId);
            return Ok(ApiResponse<IEnumerable<EventDto>>.Ok(events));
        }

        /// <summary>
        /// Marks events as read
        /// </summary>
        /// <param name="eventIds">List of event IDs to mark as read</param>
        /// <returns>Success message</returns>
        [HttpPatch("mark-read")]
        [RequirePermission("ViewEvents")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> MarkEventsAsRead([FromBody] List<int> eventIds)
        {
            var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
            await _eventService.MarkEventsAsReadAsync(eventIds, userId);
            return Ok(ApiResponse<object>.OkWithMessage($"Marked {eventIds.Count} events as read"));
        }

        /// <summary>
        /// Creates a new event (for system notifications)
        /// </summary>
        /// <param name="createDto">Event data</param>
        /// <returns>Created event</returns>
        [HttpPost]
        [RequirePermission("ManageEvents")]
        [ProducesResponseType(typeof(ApiResponse<EventDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateDto createDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                createDto.UserId = userId;
                
                var eventDto = await _eventService.CreateEventAsync(createDto);
                return CreatedAtAction(nameof(GetEvents), new { id = eventDto.Id }, 
                    ApiResponse<EventDto>.Ok(eventDto));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
        }

        /// <summary>
        /// Gets event statistics
        /// </summary>
        /// <returns>Event statistics</returns>
        [HttpGet("statistics")]
        [RequirePermission("ViewEvents")]
        [ProducesResponseType(typeof(ApiResponse<EventStatsDto>), 200)]
        public async Task<IActionResult> GetEventStatistics()
        {
            var stats = await _eventService.GetEventStatisticsAsync();
            return Ok(ApiResponse<EventStatsDto>.Ok(stats));
        }
    }
}