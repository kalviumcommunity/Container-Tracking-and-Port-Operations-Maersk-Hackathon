using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents an event in the port operations system for real-time streaming
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        [Key]
        public int EventId { get; set; }
        
        /// <summary>
        /// Type of event (Container Arrival, Ship Departure, Berth Assignment, etc.)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string EventType { get; set; } = string.Empty;
        
        /// <summary>
        /// Event category (Container, Ship, Berth, Port, System)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;
        
        /// <summary>
        /// Priority level (Critical, High, Medium, Low)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Priority { get; set; } = string.Empty;
        
        /// <summary>
        /// Current status (New, Acknowledged, In Progress, Resolved, Closed)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Event title or summary
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Detailed description of the event
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Source system or component that generated the event
        /// </summary>
        [MaxLength(100)]
        public string Source { get; set; } = string.Empty;
        
        /// <summary>
        /// Related container ID (if applicable)
        /// </summary>
        public string? ContainerId { get; set; }
        
        /// <summary>
        /// Related ship ID (if applicable)
        /// </summary>
        public int? ShipId { get; set; }
        
        /// <summary>
        /// Related berth ID (if applicable)
        /// </summary>
        public int? BerthId { get; set; }
        
        /// <summary>
        /// Related port ID (if applicable)
        /// </summary>
        public int? PortId { get; set; }
        
        /// <summary>
        /// User ID who should be notified (if specific)
        /// </summary>
        public int? AssignedToUserId { get; set; }
        
        /// <summary>
        /// User ID who acknowledged the event
        /// </summary>
        public int? AcknowledgedByUserId { get; set; }
        
        /// <summary>
        /// When the event was acknowledged
        /// </summary>
        public DateTime? AcknowledgedAt { get; set; }
        
        /// <summary>
        /// Event data as JSON for flexible storage
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string EventData { get; set; } = "{}";
        
        /// <summary>
        /// Metadata about the event as JSON
        /// </summary>
        [Column(TypeName = "jsonb")]
        public string Metadata { get; set; } = "{}";
        
        /// <summary>
        /// GPS coordinates where the event occurred (if applicable)
        /// </summary>
        public string Coordinates { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether this event requires immediate attention
        /// </summary>
        public bool RequiresAction { get; set; }
        
        /// <summary>
        /// Whether this event has been resolved
        /// </summary>
        public bool IsResolved { get; set; }
        
        /// <summary>
        /// Severity level (Critical, High, Medium, Low)
        /// </summary>
        [MaxLength(20)]
        public string Severity { get; set; } = string.Empty;
        
        /// <summary>
        /// When the event occurred
        /// </summary>
        [Required]
        public DateTime EventTimestamp { get; set; }
        
        /// <summary>
        /// When the event record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the event record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// Related container (if applicable)
        /// </summary>
        [ForeignKey("ContainerId")]
        public Container? Container { get; set; }
        
        /// <summary>
        /// Related ship (if applicable)
        /// </summary>
        [ForeignKey("ShipId")]
        public Ship? Ship { get; set; }
        
        /// <summary>
        /// Related berth (if applicable)
        /// </summary>
        [ForeignKey("BerthId")]
        public Berth? Berth { get; set; }
        
        /// <summary>
        /// Related port (if applicable)
        /// </summary>
        [ForeignKey("PortId")]
        public Port? Port { get; set; }
        
        /// <summary>
        /// User assigned to handle this event
        /// </summary>
        [ForeignKey("AssignedToUserId")]
        public User? AssignedToUser { get; set; }
        
        /// <summary>
        /// User who acknowledged this event
        /// </summary>
        [ForeignKey("AcknowledgedByUserId")]
        public User? AcknowledgedByUser { get; set; }
    }
}