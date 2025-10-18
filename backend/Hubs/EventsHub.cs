using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Backend.DTOs;
using System.Threading.Tasks;

namespace Backend.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time event broadcasting
    /// Bridges Kafka events to frontend via WebSocket connections
    /// </summary>
    [Authorize]
    public class EventsHub : Hub
    {
        private readonly ILogger<EventsHub> _logger;

        public EventsHub(ILogger<EventsHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Broadcast container event to all connected clients
        /// Called by Kafka consumer service
        /// </summary>
        public async Task BroadcastContainerEvent(EventDto eventData)
        {
            await Clients.All.SendAsync("ContainerEvent", eventData);
            _logger.LogDebug("Broadcasted container event {EventId} to all clients", eventData.EventId);
        }

        /// <summary>
        /// Broadcast berth event to all connected clients
        /// Called by Kafka consumer service
        /// </summary>
        public async Task BroadcastBerthEvent(EventDto eventData)
        {
            await Clients.All.SendAsync("BerthEvent", eventData);
            _logger.LogDebug("Broadcasted berth event {EventId} to all clients", eventData.EventId);
        }

        /// <summary>
        /// Broadcast port event to all connected clients
        /// Called by Kafka consumer service
        /// </summary>
        public async Task BroadcastPortEvent(EventDto eventData)
        {
            await Clients.All.SendAsync("PortEvent", eventData);
            _logger.LogDebug("Broadcasted port event {EventId} to all clients", eventData.EventId);
        }

        /// <summary>
        /// Broadcast alert to all connected clients
        /// Called by Kafka consumer service for critical/high severity events
        /// </summary>
        public async Task BroadcastAlert(EventDto eventData)
        {
            await Clients.All.SendAsync("Alert", eventData);
            _logger.LogInformation("Broadcasted alert {EventId} (Severity: {Severity})", 
                eventData.EventId, eventData.Severity);
        }

        /// <summary>
        /// Broadcast analytics update to all connected clients
        /// Called periodically or when significant data changes
        /// </summary>
        public async Task BroadcastAnalyticsUpdate(DashboardStatsDto stats)
        {
            await Clients.All.SendAsync("AnalyticsUpdate", stats);
            _logger.LogDebug("Broadcasted analytics update to all clients");
        }
    }
}
