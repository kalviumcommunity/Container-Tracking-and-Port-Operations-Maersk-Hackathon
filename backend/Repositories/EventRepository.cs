using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for Event entity
    /// Provides data access methods for event operations
    /// </summary>
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get events by event type
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsByTypeAsync(string eventType)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.EventType == eventType)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events by category
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.Category == category)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events by priority level
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsByPriorityAsync(string priority)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.Priority == priority)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events by severity level
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsBySeverityAsync(string severity)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.Severity == severity)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get unresolved events
        /// </summary>
        public async Task<IEnumerable<Event>> GetUnresolvedEventsAsync()
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => !e.IsResolved)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events requiring action
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsRequiringActionAsync()
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.RequiresAction && !e.IsResolved)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get most recent events
        /// </summary>
        public async Task<IEnumerable<Event>> GetRecentEventsAsync(int count = 10)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .OrderByDescending(e => e.EventTimestamp)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Get events assigned to a specific user
        /// </summary>
        public async Task<IEnumerable<Event>> GetUserAssignedEventsAsync(int userId)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.AssignedToUserId == userId)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events for a specific container
        /// </summary>
        public async Task<IEnumerable<Event>> GetContainerEventsAsync(string containerId)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.ContainerId == containerId)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events for a specific ship
        /// </summary>
        public async Task<IEnumerable<Event>> GetShipEventsAsync(int shipId)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.ShipId == shipId)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events for a specific berth
        /// </summary>
        public async Task<IEnumerable<Event>> GetBerthEventsAsync(int berthId)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.BerthId == berthId)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events for a specific port
        /// </summary>
        public async Task<IEnumerable<Event>> GetPortEventsAsync(int portId)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.PortId == portId)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get events within a date range
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Events
                .Include(e => e.Container)
                .Include(e => e.Ship)
                .Include(e => e.Berth)
                .Include(e => e.Port)
                .Include(e => e.AssignedToUser)
                .Include(e => e.AcknowledgedByUser)
                .Where(e => e.EventTimestamp >= startDate && e.EventTimestamp <= endDate)
                .OrderByDescending(e => e.EventTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get event statistics by type
        /// </summary>
        public async Task<Dictionary<string, int>> GetEventStatsByTypeAsync()
        {
            return await _context.Events
                .GroupBy(e => e.EventType)
                .Select(g => new { EventType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EventType, x => x.Count);
        }

        /// <summary>
        /// Get event statistics by severity
        /// </summary>
        public async Task<Dictionary<string, int>> GetEventStatsBySeverityAsync()
        {
            return await _context.Events
                .GroupBy(e => e.Severity)
                .Select(g => new { Severity = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Severity, x => x.Count);
        }

        /// <summary>
        /// Get event statistics by priority
        /// </summary>
        public async Task<Dictionary<string, int>> GetEventStatsByPriorityAsync()
        {
            return await _context.Events
                .GroupBy(e => e.Priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Priority, x => x.Count);
        }

        /// <summary>
        /// Get event statistics by status
        /// </summary>
        public async Task<Dictionary<string, int>> GetEventStatsByStatusAsync()
        {
            return await _context.Events
                .GroupBy(e => e.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }
    }
}
