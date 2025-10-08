using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for event information
    /// </summary>
    public class EventDto
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventTime { get; set; }
        public string Severity { get; set; }
        public string? ContainerId { get; set; }
        public int? ShipId { get; set; }
        public string? ShipName { get; set; }
        public int? BerthId { get; set; }
        public string? BerthName { get; set; }
        public int? PortId { get; set; }
        public string? PortName { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string Source { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}