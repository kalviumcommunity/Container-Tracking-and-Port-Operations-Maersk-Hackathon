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
        public string EventType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventTime { get; set; }
        public string Severity { get; set; }
        public string ContainerId { get; set; }
        public int? ShipId { get; set; }
        public string ShipName { get; set; }
        public int? BerthId { get; set; }
        public string BerthName { get; set; }
        public int? PortId { get; set; }
        public string PortName { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Source { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventUpdateDto
    {
        public string EventType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
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