using Backend.Models;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for Event entity operations
    /// Extends generic repository with event-specific methods
    /// </summary>
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Get events by event type
        /// </summary>
        /// <param name="eventType">Event type to filter by</param>
        /// <returns>Collection of events matching the type</returns>
        Task<IEnumerable<Event>> GetEventsByTypeAsync(string eventType);
        
        /// <summary>
        /// Get events by category
        /// </summary>
        /// <param name="category">Event category to filter by</param>
        /// <returns>Collection of events in the category</returns>
        Task<IEnumerable<Event>> GetEventsByCategoryAsync(string category);
        
        /// <summary>
        /// Get events by priority level
        /// </summary>
        /// <param name="priority">Priority level to filter by</param>
        /// <returns>Collection of events with specified priority</returns>
        Task<IEnumerable<Event>> GetEventsByPriorityAsync(string priority);
        
        /// <summary>
        /// Get events by severity level
        /// </summary>
        /// <param name="severity">Severity level to filter by</param>
        /// <returns>Collection of events with specified severity</returns>
        Task<IEnumerable<Event>> GetEventsBySeverityAsync(string severity);
        
        /// <summary>
        /// Get unresolved events
        /// </summary>
        /// <returns>Collection of unresolved events</returns>
        Task<IEnumerable<Event>> GetUnresolvedEventsAsync();
        
        /// <summary>
        /// Get events requiring action
        /// </summary>
        /// <returns>Collection of events that require action</returns>
        Task<IEnumerable<Event>> GetEventsRequiringActionAsync();
        
        /// <summary>
        /// Get most recent events
        /// </summary>
        /// <param name="count">Number of events to retrieve</param>
        /// <returns>Collection of most recent events</returns>
        Task<IEnumerable<Event>> GetRecentEventsAsync(int count = 10);
        
        /// <summary>
        /// Get events assigned to a specific user
        /// </summary>
        /// <param name="userId">User ID to filter by</param>
        /// <returns>Collection of events assigned to the user</returns>
        Task<IEnumerable<Event>> GetUserAssignedEventsAsync(int userId);
        
        /// <summary>
        /// Get events for a specific container
        /// </summary>
        /// <param name="containerId">Container ID to filter by</param>
        /// <returns>Collection of events related to the container</returns>
        Task<IEnumerable<Event>> GetContainerEventsAsync(string containerId);
        
        /// <summary>
        /// Get events for a specific ship
        /// </summary>
        /// <param name="shipId">Ship ID to filter by</param>
        /// <returns>Collection of events related to the ship</returns>
        Task<IEnumerable<Event>> GetShipEventsAsync(int shipId);
        
        /// <summary>
        /// Get events for a specific berth
        /// </summary>
        /// <param name="berthId">Berth ID to filter by</param>
        /// <returns>Collection of events related to the berth</returns>
        Task<IEnumerable<Event>> GetBerthEventsAsync(int berthId);
        
        /// <summary>
        /// Get events for a specific port
        /// </summary>
        /// <param name="portId">Port ID to filter by</param>
        /// <returns>Collection of events related to the port</returns>
        Task<IEnumerable<Event>> GetPortEventsAsync(int portId);
        
        /// <summary>
        /// Get events within a date range
        /// </summary>
        /// <param name="startDate">Start date of the range</param>
        /// <param name="endDate">End date of the range</param>
        /// <returns>Collection of events within the date range</returns>
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Get event statistics by type
        /// </summary>
        /// <returns>Dictionary with event type counts</returns>
        Task<Dictionary<string, int>> GetEventStatsByTypeAsync();
        
        /// <summary>
        /// Get event statistics by severity
        /// </summary>
        /// <returns>Dictionary with event severity counts</returns>
        Task<Dictionary<string, int>> GetEventStatsBySeverityAsync();
        
        /// <summary>
        /// Get event statistics by priority
        /// </summary>
        /// <returns>Dictionary with event priority counts</returns>
        Task<Dictionary<string, int>> GetEventStatsByPriorityAsync();
        
        /// <summary>
        /// Get event statistics by status
        /// </summary>
        /// <returns>Dictionary with event status counts</returns>
        Task<Dictionary<string, int>> GetEventStatsByStatusAsync();
    }
}
