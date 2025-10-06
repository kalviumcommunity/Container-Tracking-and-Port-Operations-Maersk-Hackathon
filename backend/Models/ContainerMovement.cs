using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents a movement/tracking record for a container
    /// </summary>
    public class ContainerMovement
    {
        /// <summary>
        /// Unique identifier for the movement record
        /// </summary>
        [Key]
        public int MovementId { get; set; }
        
        /// <summary>
        /// Foreign key to the container being tracked
        /// </summary>
        [Required]
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Type of movement (Loading, Unloading, Transfer, Transit, Arrival, Departure)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string MovementType { get; set; }
        
        /// <summary>
        /// Source location (port, ship, berth, etc.)
        /// </summary>
        [MaxLength(200)]
        public string FromLocation { get; set; }
        
        /// <summary>
        /// Destination location (port, ship, berth, etc.)
        /// </summary>
        [MaxLength(200)]
        public string ToLocation { get; set; }
        
        /// <summary>
        /// Current status of the movement (In Progress, Completed, Cancelled, Delayed)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; }
        
        /// <summary>
        /// GPS coordinates at the time of movement
        /// </summary>
        public string Coordinates { get; set; }
        
        /// <summary>
        /// Estimated completion time
        /// </summary>
        public DateTime? EstimatedCompletion { get; set; }
        
        /// <summary>
        /// Actual completion time
        /// </summary>
        public DateTime? ActualCompletion { get; set; }
        
        /// <summary>
        /// Duration of the movement in minutes
        /// </summary>
        public int? DurationMinutes { get; set; }
        
        /// <summary>
        /// Distance traveled in kilometers
        /// </summary>
        public decimal? DistanceKm { get; set; }
        
        /// <summary>
        /// Associated ship for this movement (if applicable)
        /// </summary>
        public int? ShipId { get; set; }
        
        /// <summary>
        /// Associated berth for this movement (if applicable)
        /// </summary>
        public int? BerthId { get; set; }
        
        /// <summary>
        /// Associated port for this movement (if applicable)
        /// </summary>
        public int? PortId { get; set; }
        
        /// <summary>
        /// Temperature reading (for refrigerated containers)
        /// </summary>
        public decimal? Temperature { get; set; }
        
        /// <summary>
        /// Humidity reading (for climate-controlled containers)
        /// </summary>
        public decimal? Humidity { get; set; }
        
        /// <summary>
        /// Additional notes or observations
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// User who recorded this movement
        /// </summary>
        public int? RecordedByUserId { get; set; }
        
        /// <summary>
        /// When the movement started
        /// </summary>
        [Required]
        public DateTime MovementTimestamp { get; set; }
        
        /// <summary>
        /// When this record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When this record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The container being tracked
        /// </summary>
        [ForeignKey("ContainerId")]
        public Container Container { get; set; }
        
        /// <summary>
        /// Associated ship (if applicable)
        /// </summary>
        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
        
        /// <summary>
        /// Associated berth (if applicable)
        /// </summary>
        [ForeignKey("BerthId")]
        public Berth Berth { get; set; }
        
        /// <summary>
        /// Associated port (if applicable)
        /// </summary>
        [ForeignKey("PortId")]
        public Port Port { get; set; }
        
        /// <summary>
        /// User who recorded this movement
        /// </summary>
        [ForeignKey("RecordedByUserId")]
        public User RecordedByUser { get; set; }
    }
}