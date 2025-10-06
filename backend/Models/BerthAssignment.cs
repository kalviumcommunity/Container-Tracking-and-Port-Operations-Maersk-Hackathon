using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents an assignment of a ship/container to a berth for a specific time period
    /// </summary>
    public class BerthAssignment
    {
        /// <summary>
        /// Unique identifier for the berth assignment
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key to the berth being assigned
        /// </summary>
        [Required]
        public int BerthId { get; set; }
        
        /// <summary>
        /// Foreign key to the ship assigned to the berth
        /// </summary>
        public int? ShipId { get; set; }
        
        /// <summary>
        /// Foreign key to the container assigned to the berth (optional)
        /// </summary>
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Type of assignment (Loading, Unloading, Storage, Maintenance)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string AssignmentType { get; set; }
        
        /// <summary>
        /// Priority level (High, Medium, Low)
        /// </summary>
        [MaxLength(20)]
        public string Priority { get; set; }
        
        /// <summary>
        /// Current status (Scheduled, Active, Completed, Cancelled)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
        
        /// <summary>
        /// Scheduled arrival time at the berth
        /// </summary>
        public DateTime? ScheduledArrival { get; set; }
        
        /// <summary>
        /// Actual arrival time at the berth
        /// </summary>
        public DateTime? ActualArrival { get; set; }
        
        /// <summary>
        /// Scheduled departure time from the berth
        /// </summary>
        public DateTime? ScheduledDeparture { get; set; }
        
        /// <summary>
        /// Actual departure time from the berth
        /// </summary>
        public DateTime? ActualDeparture { get; set; }
        
        /// <summary>
        /// Time when the assignment was created
        /// </summary>
        [Required]
        public DateTime AssignedAt { get; set; }
        
        /// <summary>
        /// Time when the assignment was completed/released
        /// </summary>
        public DateTime? ReleasedAt { get; set; }
        
        /// <summary>
        /// Number of containers being processed (for ship assignments)
        /// </summary>
        public int? ContainerCount { get; set; }
        
        /// <summary>
        /// Estimated processing time in hours
        /// </summary>
        public decimal? EstimatedProcessingTime { get; set; }
        
        /// <summary>
        /// Actual processing time in hours
        /// </summary>
        public decimal? ActualProcessingTime { get; set; }
        
        /// <summary>
        /// Cost of the berth assignment
        /// </summary>
        public decimal? Cost { get; set; }
        
        /// <summary>
        /// Additional notes or special instructions
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// User who created the assignment
        /// </summary>
        public int? CreatedByUserId { get; set; }
        
        /// <summary>
        /// Timestamp when the assignment was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Timestamp when the assignment was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The berth being assigned
        /// </summary>
        [ForeignKey("BerthId")]
        public Berth Berth { get; set; }
        
        /// <summary>
        /// The ship assigned to the berth (if any)
        /// </summary>
        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
        
        /// <summary>
        /// The container assigned to the berth (if any)
        /// </summary>
        [ForeignKey("ContainerId")]
        public Container Container { get; set; }
        
        /// <summary>
        /// The user who created the assignment
        /// </summary>
        [ForeignKey("CreatedByUserId")]
        public User CreatedBy { get; set; }
    }
}