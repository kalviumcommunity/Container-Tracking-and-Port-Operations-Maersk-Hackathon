using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using Backend.Services.Kafka;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Interface for event service operations
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Get filtered events with pagination
        /// </summary>
        Task<PaginatedResponse<EventDto>> GetFilteredEventsAsync(EventFilterDto filter);

        /// <summary>
        /// Get event by ID
        /// </summary>
        Task<EventDto?> GetByIdAsync(int eventId);

        /// <summary>
        /// Create a new event
        /// </summary>
        Task<EventDto> CreateAsync(EventCreateDto createDto);

        /// <summary>
        /// Update an existing event
        /// </summary>
        Task<EventDto> UpdateAsync(int eventId, EventUpdateDto updateDto);

        /// <summary>
        /// Delete an event
        /// </summary>
        Task<bool> DeleteAsync(int eventId);

        /// <summary>
        /// Get events by type
        /// </summary>
        Task<IEnumerable<EventDto>> GetEventsByTypeAsync(string eventType);

        /// <summary>
        /// Get events by priority
        /// </summary>
        Task<IEnumerable<EventDto>> GetEventsByPriorityAsync(string priority);

        /// <summary>
        /// Get unresolved events
        /// </summary>
        Task<IEnumerable<EventDto>> GetUnresolvedEventsAsync();

        /// <summary>
        /// Get recent events
        /// </summary>
        Task<IEnumerable<EventDto>> GetRecentEventsAsync(int count);

        /// <summary>
        /// Get unread events for a user
        /// </summary>
        Task<IEnumerable<EventDto>> GetUnreadEventsAsync(int userId);

        /// <summary>
        /// Acknowledge an event
        /// </summary>
        Task<EventDto> AcknowledgeAsync(int eventId, int userId);

        /// <summary>
        /// Resolve an event
        /// </summary>
        Task<EventDto> ResolveAsync(int eventId, int userId, string resolution);

        /// <summary>
        /// Get user's assigned events
        /// </summary>
        Task<IEnumerable<EventDto>> GetUserAssignedEventsAsync(int userId);

        /// <summary>
        /// Mark multiple events as read
        /// </summary>
        Task<bool> MarkEventsAsReadAsync(List<int> eventIds, int userId);

        /// <summary>
        /// Get events requiring action
        /// </summary>
        Task<IEnumerable<EventDto>> GetEventsRequiringActionAsync();

        /// <summary>
        /// Get event statistics by type
        /// </summary>
        Task<Dictionary<string, int>> GetEventStatsByTypeAsync();

        /// <summary>
        /// Get event statistics by priority
        /// </summary>
        Task<Dictionary<string, int>> GetEventStatsByPriorityAsync();

        /// <summary>
        /// Get comprehensive event statistics
        /// </summary>
        Task<EventStatsDto> GetEventStatisticsAsync();
    }

    /// <summary>
    /// Implementation of Event service for managing port operation events
    /// </summary>
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventService> _logger;
        private readonly IKafkaProducer _kafkaProducer;

        public EventService(
            ApplicationDbContext context, 
            IEventRepository eventRepository,
            ILogger<EventService> logger, 
            IKafkaProducer kafkaProducer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        }

        /// <summary>
        /// Get filtered events with pagination
        /// </summary>
        public async Task<PaginatedResponse<EventDto>> GetFilteredEventsAsync(EventFilterDto filter)
        {
            try
            {
                filter ??= new EventFilterDto();
                filter.Page = Math.Max(1, filter.Page);
                filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);

                var query = _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .AsNoTracking()
                    .AsQueryable();

                // Apply filters
                query = ApplyFilters(query, filter);

                // Apply sorting
                query = ApplySorting(query, filter);

                var totalCount = await query.CountAsync();
                var events = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Select(ProjectToDto())
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);

                return new PaginatedResponse<EventDto>
                {
                    Data = events,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = filter.Page < totalPages,
                    HasPreviousPage = filter.Page > 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered events");
                throw;
            }
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        public async Task<EventDto?> GetByIdAsync(int eventId)
        {
            try
            {
                var eventEntity = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Include(e => e.AcknowledgedByUser)
                    .FirstOrDefaultAsync(e => e.EventId == eventId);

                if (eventEntity == null)
                {
                    return null;
                }

                return MapToDto(eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        public async Task<EventDto> CreateAsync(EventCreateDto createDto)
        {
            try
            {
                // Fix: Use Severity instead of Priority
                var priority = !string.IsNullOrWhiteSpace(createDto.Severity)
                    ? createDto.Severity
                    : "Medium"; // Default severity

                var eventEntity = new Event
                {
                    EventType = createDto.EventType,
                    Title = createDto.Title,
                    Description = createDto.Description,
                    EventTimestamp = createDto.EventTime ?? DateTime.UtcNow,
                    Priority = priority, // Set priority from severity
                    ContainerId = createDto.ContainerId,
                    ShipId = createDto.ShipId,
                    BerthId = createDto.BerthId,
                    PortId = createDto.PortId,
                    AssignedToUserId = createDto.UserId,
                    Source = createDto.Source,
                    EventData = string.IsNullOrWhiteSpace(createDto.AdditionalData) ? "{}" : createDto.AdditionalData,
                    Category = "General", // Default category since it's not in DTO
                    Status = "New",
                    RequiresAction = priority == "Critical" || priority == "High",
                    IsResolved = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Events.Add(eventEntity);
                await _context.SaveChangesAsync();

                // Load navigation properties for response
                await LoadNavigationProperties(eventEntity);

                var dto = MapToDto(eventEntity);

                // Publish to Kafka (best-effort; log failures without failing API)
                try
                {
                    var settings = new Backend.Services.Kafka.KafkaSettings(); // topic names are bound elsewhere; pick by category/type
                    var topic = (eventEntity.ContainerId != null) ? "container-events" : "port-events";
                    var key = eventEntity.ContainerId?.ToString() ?? eventEntity.PortId?.ToString() ?? eventEntity.EventId.ToString();
                    var payload = new
                    {
                        id = eventEntity.EventId,
                        eventId = eventEntity.EventId,
                        eventType = eventEntity.EventType,
                        title = eventEntity.Title,
                        description = eventEntity.Description,
                        status = eventEntity.Status,
                        timestamp = eventEntity.EventTimestamp,
                        eventTime = eventEntity.EventTimestamp,
                        source = eventEntity.Source,
                        portId = eventEntity.PortId,
                        shipId = eventEntity.ShipId,
                        containerId = eventEntity.ContainerId,
                        berthId = eventEntity.BerthId,
                        priority = eventEntity.Priority,
                        severity = eventEntity.Priority,  // Add severity field for email service
                        requiresAction = eventEntity.RequiresAction
                    };
                    var json = JsonSerializer.Serialize(payload);
                    await _kafkaProducer.PublishAsync(topic, key ?? eventEntity.EventId.ToString(), json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish event {EventId} to Kafka", eventEntity.EventId);
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                throw;
            }
        }

        /// <summary>
        /// Update an existing event
        /// </summary>
        public async Task<EventDto> UpdateAsync(int eventId, EventUpdateDto updateDto)
        {
            try
            {
                var eventEntity = await _context.Events.FindAsync(eventId);
                if (eventEntity == null)
                {
                    throw new KeyNotFoundException($"Event with ID {eventId} not found");
                }

                // Update properties
                if (!string.IsNullOrEmpty(updateDto.EventType))
                    eventEntity.EventType = updateDto.EventType;

                if (!string.IsNullOrEmpty(updateDto.Title))
                    eventEntity.Title = updateDto.Title;

                if (!string.IsNullOrEmpty(updateDto.Description))
                    eventEntity.Description = updateDto.Description;

                if (!string.IsNullOrEmpty(updateDto.Priority))
                    eventEntity.Priority = updateDto.Priority;

                if (!string.IsNullOrEmpty(updateDto.Status))
                    eventEntity.Status = updateDto.Status;

                if (updateDto.AssignedToUserId.HasValue)
                    eventEntity.AssignedToUserId = updateDto.AssignedToUserId;

                eventEntity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await LoadNavigationProperties(eventEntity);
                return MapToDto(eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        public async Task<bool> DeleteAsync(int eventId)
        {
            try
            {
                var eventEntity = await _context.Events.FindAsync(eventId);
                if (eventEntity == null)
                {
                    return false;
                }

                _context.Events.Remove(eventEntity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Get events by type
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetEventsByTypeAsync(string eventType)
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Where(e => e.EventType == eventType)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events by type {EventType}", eventType);
                throw;
            }
        }

        /// <summary>
        /// Get events by priority
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetEventsByPriorityAsync(string priority)
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Where(e => e.Priority == priority)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events by priority {Priority}", priority);
                throw;
            }
        }

        /// <summary>
        /// Get unresolved events
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetUnresolvedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Where(e => !e.IsResolved)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unresolved events");
                throw;
            }
        }

        /// <summary>
        /// Get recent events
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetRecentEventsAsync(int count)
        {
            try
            {
                count = Math.Clamp(count, 1, 100);

                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Take(count)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent events");
                throw;
            }
        }

        /// <summary>
        /// Get unread events for a user
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetUnreadEventsAsync(int userId)
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Where(e => e.AssignedToUserId == userId && !e.IsResolved)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread events for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Acknowledge an event
        /// </summary>
        public async Task<EventDto> AcknowledgeAsync(int eventId, int userId)
        {
            try
            {
                var eventEntity = await _context.Events.FindAsync(eventId);
                if (eventEntity == null)
                {
                    throw new KeyNotFoundException($"Event with ID {eventId} not found");
                }

                eventEntity.AcknowledgedByUserId = userId;
                eventEntity.AcknowledgedAt = DateTime.UtcNow;
                eventEntity.Status = "Acknowledged";
                eventEntity.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await LoadNavigationProperties(eventEntity);

                return MapToDto(eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acknowledging event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Resolve an event
        /// </summary>
        public async Task<EventDto> ResolveAsync(int eventId, int userId, string resolution)
        {
            try
            {
                var eventEntity = await _context.Events.FindAsync(eventId);
                if (eventEntity == null)
                {
                    throw new KeyNotFoundException($"Event with ID {eventId} not found");
                }

                eventEntity.IsResolved = true;
                eventEntity.Status = "Resolved";
                eventEntity.UpdatedAt = DateTime.UtcNow;

                // If not already acknowledged, acknowledge by the same user
                if (eventEntity.AcknowledgedByUserId == null)
                {
                    eventEntity.AcknowledgedByUserId = userId;
                    eventEntity.AcknowledgedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await LoadNavigationProperties(eventEntity);

                return MapToDto(eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving event {EventId}", eventId);
                throw;
            }
        }

        /// <summary>
        /// Get user's assigned events
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetUserAssignedEventsAsync(int userId)
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Where(e => e.AssignedToUserId == userId)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assigned events for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Mark multiple events as read
        /// </summary>
        public async Task<bool> MarkEventsAsReadAsync(List<int> eventIds, int userId)
        {
            try
            {
                if (eventIds == null || eventIds.Count == 0)
                {
                    return true;
                }

                var events = await _context.Events
                    .Where(e => eventIds.Contains(e.EventId) && e.AssignedToUserId == userId)
                    .ToListAsync();

                foreach (var eventEntity in events)
                {
                    // mark as read, do not change resolution status unless intended
                    eventEntity.AcknowledgedAt = DateTime.UtcNow;
                    eventEntity.AcknowledgedByUserId = userId;
                    eventEntity.Status = eventEntity.Status ?? "Acknowledged";
                    eventEntity.UpdatedAt = DateTime.UtcNow;
                }

                if (events.Count > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking events as read for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Get events requiring action
        /// </summary>
        public async Task<IEnumerable<EventDto>> GetEventsRequiringActionAsync()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Ship)
                    .Include(e => e.Berth)
                    .Include(e => e.Port)
                    .Include(e => e.AssignedToUser)
                    .Where(e => e.RequiresAction && !e.IsResolved)
                    .OrderByDescending(e => e.EventTimestamp)
                    .Select(ProjectToDto())
                    .ToListAsync();

                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events requiring action");
                throw;
            }
        }

        /// <summary>
        /// Get event statistics by type
        /// </summary>
        public async Task<Dictionary<string, int>> GetEventStatsByTypeAsync()
        {
            try
            {
                var stats = await _context.Events
                    .GroupBy(e => e.EventType)
                    .Select(g => new { EventType = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.EventType ?? "Unknown", x => x.Count);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event stats by type");
                throw;
            }
        }

        /// <summary>
        /// Get event statistics by priority
        /// </summary>
        public async Task<Dictionary<string, int>> GetEventStatsByPriorityAsync()
        {
            try
            {
                var stats = await _context.Events
                    .GroupBy(e => e.Priority)
                    .Select(g => new { Priority = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Priority ?? "Unknown", x => x.Count);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event stats by priority");
                throw;
            }
        }

        /// <summary>
        /// Get comprehensive event statistics
        /// </summary>
        public async Task<EventStatsDto> GetEventStatisticsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var today = now.Date;
                var weekAgo = today.AddDays(-7);

                var totalEvents = await _context.Events.CountAsync();
                var unreadEvents = await _context.Events.CountAsync(e => !e.IsResolved);
                var todayEvents = await _context.Events.CountAsync(e => e.CreatedAt.Date == today);
                var weekEvents = await _context.Events.CountAsync(e => e.CreatedAt >= weekAgo);

                var eventsBySeverity = await _context.Events
                    .GroupBy(e => e.Priority)
                    .Select(g => new { Priority = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Priority ?? "Unknown", x => x.Count);

                var eventsByType = await _context.Events
                    .GroupBy(e => e.EventType)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Type ?? "Unknown", x => x.Count);

                var eventsBySource = await _context.Events
                    .GroupBy(e => e.Source)
                    .Select(g => new { Source = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Source ?? "Unknown", x => x.Count);

                var recentTrends = await _context.Events
                    .Where(e => e.CreatedAt >= weekAgo)
                    .GroupBy(e => new { Date = e.CreatedAt.Date, e.EventType })
                    .Select(g => new EventTrendDto
                    {
                        Date = g.Key.Date,
                        Count = g.Count(),
                        EventType = g.Key.EventType
                    })
                    .OrderBy(t => t.Date)
                    .ToListAsync();

                return new EventStatsDto
                {
                    TotalEvents = totalEvents,
                    UnreadEvents = unreadEvents,
                    TodayEvents = todayEvents,
                    WeekEvents = weekEvents,
                    EventsBySeverity = eventsBySeverity,
                    EventsByType = eventsByType,
                    EventsBySource = eventsBySource,
                    RecentTrends = recentTrends
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event statistics");
                throw;
            }
        }

        #region Private Helper Methods

        private IQueryable<Event> ApplyFilters(IQueryable<Event> query, EventFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.EventType))
                query = query.Where(e => EF.Functions.ILike(e.EventType, $"%{filter.EventType}%"));

            if (!string.IsNullOrWhiteSpace(filter.Severity))
                query = query.Where(e => e.Priority == filter.Severity);

            if (!string.IsNullOrWhiteSpace(filter.Source))
                query = query.Where(e => e.Source == filter.Source);

            if (!string.IsNullOrWhiteSpace(filter.ContainerId))
                query = query.Where(e => e.ContainerId == filter.ContainerId);

            if (filter.ShipId.HasValue)
                query = query.Where(e => e.ShipId == filter.ShipId.Value);

            if (filter.BerthId.HasValue)
                query = query.Where(e => e.BerthId == filter.BerthId.Value);

            if (filter.PortId.HasValue)
                query = query.Where(e => e.PortId == filter.PortId.Value);

            if (filter.UserId.HasValue)
                query = query.Where(e => e.AssignedToUserId == filter.UserId.Value);

            if (filter.EventAfter.HasValue)
                query = query.Where(e => e.EventTimestamp >= filter.EventAfter.Value);

            if (filter.EventBefore.HasValue)
                query = query.Where(e => e.EventTimestamp <= filter.EventBefore.Value);

            // IsRead should map to IsRead (not IsResolved)
            if (filter.IsRead.HasValue)
                query = query.Where(e => (e.AcknowledgedAt.HasValue) == filter.IsRead.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(e =>
                    EF.Functions.ILike(e.Title, $"%{term}%") ||
                    EF.Functions.ILike(e.Description, $"%{term}%") ||
                    EF.Functions.ILike(e.EventType, $"%{term}%"));
            }

            return query;
        }

        private IQueryable<Event> ApplySorting(IQueryable<Event> query, EventFilterDto filter)
        {
            var sortBy = filter.SortBy?.ToLowerInvariant() ?? "eventtimestamp";
            var sortDirection = filter.SortDirection?.ToLowerInvariant() == "asc" ? "asc" : "desc";

            return sortBy switch
            {
                "title" => sortDirection == "asc" ? query.OrderBy(e => e.Title) : query.OrderByDescending(e => e.Title),
                "eventtype" => sortDirection == "asc" ? query.OrderBy(e => e.EventType) : query.OrderByDescending(e => e.EventType),
                "priority" => sortDirection == "asc" ? query.OrderBy(e => e.Priority) : query.OrderByDescending(e => e.Priority),
                "createdat" => sortDirection == "asc" ? query.OrderBy(e => e.CreatedAt) : query.OrderByDescending(e => e.CreatedAt),
                _ => sortDirection == "asc" ? query.OrderBy(e => e.EventTimestamp) : query.OrderByDescending(e => e.EventTimestamp)
            };
        }

        private async Task LoadNavigationProperties(Event eventEntity)
        {
            await _context.Entry(eventEntity).Reference(e => e.Ship).LoadAsync();
            await _context.Entry(eventEntity).Reference(e => e.Berth).LoadAsync();
            await _context.Entry(eventEntity).Reference(e => e.Port).LoadAsync();
            await _context.Entry(eventEntity).Reference(e => e.AssignedToUser).LoadAsync();
            await _context.Entry(eventEntity).Reference(e => e.AcknowledgedByUser).LoadAsync();
        }

        private static EventDto MapToDto(Event eventEntity)
        {
            if (eventEntity == null) return null!;

            return new EventDto
            {
                Id = eventEntity.EventId,
                EventId = eventEntity.EventId, // Add for compatibility
                EventType = eventEntity.EventType,
                Title = eventEntity.Title,
                Description = eventEntity.Description,
                EventTime = eventEntity.EventTimestamp,
                Severity = eventEntity.Priority,
                ContainerId = eventEntity.ContainerId,
                ShipId = eventEntity.ShipId,
                ShipName = eventEntity.Ship?.Name,
                BerthId = eventEntity.BerthId,
                BerthName = eventEntity.Berth?.Name,
                PortId = eventEntity.PortId,
                PortName = eventEntity.Port?.Name,
                UserId = eventEntity.AssignedToUserId,
                UserName = eventEntity.AssignedToUser?.FullName,
                Source = eventEntity.Source,
                IsRead = eventEntity.AcknowledgedAt.HasValue,
                CreatedAt = eventEntity.CreatedAt
            };
        }

        private static System.Linq.Expressions.Expression<Func<Event, EventDto>> ProjectToDto()
        {
            return e => new EventDto
            {
                Id = e.EventId,
                EventId = e.EventId, // Add for compatibility
                EventType = e.EventType,
                Title = e.Title,
                Description = e.Description,
                EventTime = e.EventTimestamp,
                Severity = e.Priority,
                ContainerId = e.ContainerId,
                ShipId = e.ShipId,
                ShipName = e.Ship != null ? e.Ship.Name : null,
                BerthId = e.BerthId,
                BerthName = e.Berth != null ? e.Berth.Name : null,
                PortId = e.PortId,
                PortName = e.Port != null ? e.Port.Name : null,
                UserId = e.AssignedToUserId,
                UserName = e.AssignedToUser != null ? e.AssignedToUser.FullName : null,
                Source = e.Source,
                IsRead = e.AcknowledgedAt.HasValue,
                CreatedAt = e.CreatedAt
            };
        }

        #endregion
    }
}
