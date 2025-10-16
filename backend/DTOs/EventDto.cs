using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for event information
    /// </summary>
    public class EventDto
    {
        public int Id { get; set; }
        public int EventId { get; set; } // Add this for compatibility
        public string EventType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Container, Ship, Berth, Port, System
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }
        public DateTime EventTimestamp { get; set; } // Alias for EventTime
        public string Severity { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ContainerId { get; set; }
        public int? ShipId { get; set; }
        public string? ShipName { get; set; }
        public int? BerthId { get; set; }
        public string? BerthName { get; set; }
        public int? PortId { get; set; }
        public string? PortName { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public string Source { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsResolved { get; set; }
        public bool RequiresAction { get; set; }
        public string EventData { get; set; } = "{}";
        public string Metadata { get; set; } = "{}";
        public string Coordinates { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public int? AcknowledgedByUserId { get; set; }
        public string? AcknowledgedByUserName { get; set; }
    }

    public class EventUpdateDto
    {
        public string EventType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
    }

    public class EventFilterDto
    {
        public string? EventType { get; set; }
        public string? Severity { get; set; }
        public string? Source { get; set; }
        public string? ContainerId { get; set; }
        public int? ShipId { get; set; }
        public int? BerthId { get; set; }
        public int? PortId { get; set; }
        public int? UserId { get; set; }
        public DateTime? EventAfter { get; set; }
        public DateTime? EventBefore { get; set; }
        public bool? IsRead { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "EventTimestamp";
        public string? SortDirection { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class EventStatsDto
    {
        public int TotalEvents { get; set; }
        public int UnreadEvents { get; set; }
        public int TodayEvents { get; set; }
        public int WeekEvents { get; set; }
        public Dictionary<string, int> EventsBySeverity { get; set; } = new();
        public Dictionary<string, int> EventsByType { get; set; } = new();
        public Dictionary<string, int> EventsBySource { get; set; } = new();
        public List<EventTrendDto> RecentTrends { get; set; } = new();
    }

    public class EventTrendDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string EventType { get; set; }
    }
}