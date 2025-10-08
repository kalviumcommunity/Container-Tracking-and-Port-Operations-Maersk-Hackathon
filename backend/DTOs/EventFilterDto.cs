using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for filtering events
    /// </summary>
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
}
