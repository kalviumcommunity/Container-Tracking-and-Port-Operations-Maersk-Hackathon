using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new event
    /// </summary>
    public class EventCreateDto
    {
        [Required]
        public string EventType { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime? EventTime { get; set; }

        [Required]
        public string Severity { get; set; } = "Medium";

        public string? ContainerId { get; set; }

        public int? ShipId { get; set; }

        public int? BerthId { get; set; }

        public int? PortId { get; set; }

        public int? UserId { get; set; }

        public string Source { get; set; } = "System";

        public string? AdditionalData { get; set; }
    }
}
