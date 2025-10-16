using Microsoft.AspNetCore.SignalR;
using Backend.DTOs;
using System.Collections.Concurrent;

namespace Backend.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time event streaming to connected clients
    /// </summary>
    public class EventHub : Hub
    {
        private readonly ILogger<EventHub> _logger;
        private static readonly ConcurrentDictionary<string, string> _connectionUserMap = new();

        public EventHub(ILogger<EventHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects to the hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name ?? "Anonymous";
            
            _connectionUserMap.TryAdd(connectionId, userId);
            
            _logger.LogInformation("Client connected to EventHub: ConnectionId={ConnectionId}, User={User}", 
                connectionId, userId);

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            _connectionUserMap.TryRemove(connectionId, out var userId);

            if (exception != null)
            {
                _logger.LogWarning(exception, "Client disconnected from EventHub with error: ConnectionId={ConnectionId}", 
                    connectionId);
            }
            else
            {
                _logger.LogInformation("Client disconnected from EventHub: ConnectionId={ConnectionId}", 
                    connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Subscribe to specific event categories
        /// </summary>
        /// <param name="categories">List of categories to subscribe to (e.g., "Container", "Ship", "Port")</param>
        public async Task SubscribeToCategories(List<string> categories)
        {
            foreach (var category in categories)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Category_{category}");
                _logger.LogInformation("Client {ConnectionId} subscribed to category: {Category}", 
                    Context.ConnectionId, category);
            }
        }

        /// <summary>
        /// Unsubscribe from specific event categories
        /// </summary>
        /// <param name="categories">List of categories to unsubscribe from</param>
        public async Task UnsubscribeFromCategories(List<string> categories)
        {
            foreach (var category in categories)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Category_{category}");
                _logger.LogInformation("Client {ConnectionId} unsubscribed from category: {Category}", 
                    Context.ConnectionId, category);
            }
        }

        /// <summary>
        /// Subscribe to specific severity levels
        /// </summary>
        /// <param name="severities">List of severities to subscribe to (e.g., "Critical", "High")</param>
        public async Task SubscribeToSeverities(List<string> severities)
        {
            foreach (var severity in severities)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Severity_{severity}");
                _logger.LogInformation("Client {ConnectionId} subscribed to severity: {Severity}", 
                    Context.ConnectionId, severity);
            }
        }

        /// <summary>
        /// Get current connection count
        /// </summary>
        public int GetConnectionCount()
        {
            return _connectionUserMap.Count;
        }
    }

    /// <summary>
    /// Interface for sending messages through EventHub from outside the hub
    /// </summary>
    public interface IEventHubService
    {
        Task BroadcastEventAsync(EventDto eventDto);
        Task BroadcastEventToCategoryAsync(string category, EventDto eventDto);
        Task BroadcastEventToSeverityAsync(string severity, EventDto eventDto);
        Task BroadcastEventStatsAsync(EventStatsDto stats);
        Task BroadcastConnectionCountAsync(int count);
    }

    /// <summary>
    /// Service for broadcasting events through SignalR EventHub
    /// </summary>
    public class EventHubService : IEventHubService
    {
        private readonly IHubContext<EventHub> _hubContext;
        private readonly ILogger<EventHubService> _logger;

        public EventHubService(IHubContext<EventHub> hubContext, ILogger<EventHubService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Broadcast event to all connected clients
        /// </summary>
        public async Task BroadcastEventAsync(EventDto eventDto)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveEvent", eventDto);
                _logger.LogDebug("Broadcasted event {EventId} to all clients", eventDto.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast event {EventId}", eventDto.EventId);
            }
        }

        /// <summary>
        /// Broadcast event to clients subscribed to specific category
        /// </summary>
        public async Task BroadcastEventToCategoryAsync(string category, EventDto eventDto)
        {
            try
            {
                var groupName = $"Category_{category}";
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveEvent", eventDto);
                _logger.LogDebug("Broadcasted event {EventId} to category group: {Category}", 
                    eventDto.EventId, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast event {EventId} to category {Category}", 
                    eventDto.EventId, category);
            }
        }

        /// <summary>
        /// Broadcast event to clients subscribed to specific severity
        /// </summary>
        public async Task BroadcastEventToSeverityAsync(string severity, EventDto eventDto)
        {
            try
            {
                var groupName = $"Severity_{severity}";
                await _hubContext.Clients.Group(groupName).SendAsync("ReceiveEvent", eventDto);
                _logger.LogDebug("Broadcasted event {EventId} to severity group: {Severity}", 
                    eventDto.EventId, severity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast event {EventId} to severity {Severity}", 
                    eventDto.EventId, severity);
            }
        }

        /// <summary>
        /// Broadcast event statistics to all connected clients
        /// </summary>
        public async Task BroadcastEventStatsAsync(EventStatsDto stats)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveEventStats", stats);
                _logger.LogDebug("Broadcasted event statistics");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast event statistics");
            }
        }

        /// <summary>
        /// Broadcast connection count update
        /// </summary>
        public async Task BroadcastConnectionCountAsync(int count)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveConnectionCount", count);
                _logger.LogDebug("Broadcasted connection count: {Count}", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to broadcast connection count");
            }
        }
    }
}
