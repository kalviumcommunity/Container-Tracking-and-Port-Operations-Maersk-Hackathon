using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for BerthAssignment
    /// </summary>
    public class BerthAssignmentDto
    {
        /// <summary>
        /// Unique identifier for the berth assignment
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key to the container assigned to the berth
        /// </summary>
        public string? ContainerId { get; set; }
        
        /// <summary>
        /// The name of the container assigned to the berth
        /// </summary>
        public string? ContainerName { get; set; }
        
        /// <summary>
        /// Foreign key to the berth the container is assigned to
        /// </summary>
        public int BerthId { get; set; }
        
        /// <summary>
        /// The name of the berth the container is assigned to
        /// </summary>
        public string? BerthName { get; set; }
        
        /// <summary>
        /// Time when the container was assigned to the berth
        /// </summary>
        public DateTime AssignedAt { get; set; }
        
        /// <summary>
        /// Time when the container was released from the berth (null if still assigned)
        /// </summary>
        public DateTime? ReleasedAt { get; set; }
        
        /// <summary>
        /// The status of the assignment (Active or Released)
        /// </summary>
        public string Status => ReleasedAt.HasValue ? "Released" : "Active";
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a BerthAssignment
    /// </summary>
    public class BerthAssignmentCreateUpdateDto
    {
        /// <summary>
        /// Foreign key to the berth being assigned
        /// </summary>
        [Required]
        public int BerthId { get; set; }
        
        /// <summary>
        /// Foreign key to the ship assigned to the berth (optional)
        /// </summary>
        public int? ShipId { get; set; }
        
        /// <summary>
        /// Foreign key to the container assigned to the berth (optional)
        /// </summary>
        public string? ContainerId { get; set; }
        
        /// <summary>
        /// Type of assignment (Ship, Container, Loading, Unloading, Storage, Maintenance)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string AssignmentType { get; set; } = string.Empty;
        
        /// <summary>
        /// Priority level (1=High, 2=Medium, 3=Low)
        /// </summary>
        public int? Priority { get; set; }
        
        /// <summary>
        /// Current status (defaults to "Scheduled")
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";
        
        /// <summary>
        /// Scheduled arrival/start time at the berth
        /// </summary>
        public DateTime? ScheduledArrival { get; set; }
        
        /// <summary>
        /// Scheduled departure/end time from the berth
        /// </summary>
        public DateTime? ScheduledDeparture { get; set; }
        
        /// <summary>
        /// Number of containers being processed or capacity required
        /// </summary>
        public int? ContainerCount { get; set; }
        
        /// <summary>
        /// Additional notes or special instructions
        /// </summary>
        public string? Notes { get; set; }
    }
}