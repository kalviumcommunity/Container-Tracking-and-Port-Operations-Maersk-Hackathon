using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents a shipping container being tracked through the system
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Unique identifier for the container using industry standard format (e.g., MSCU1234567)
        /// </summary>
        [Key]
        [Required]
        [MaxLength(11)]
        public string ContainerId { get; set; } = string.Empty;
        
        /// <summary>
        /// Type/category of cargo being transported (Electronics, Dairy, Automotive, etc.)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CargoType { get; set; } = string.Empty;
        
        /// <summary>
        /// Type of container (Dry, Refrigerated, Tank, OpenTop, FlatRack)
        /// </summary>
        [Required]
        public string Type { get; set; } = string.Empty;
        
        /// <summary>
        /// Current status of the container (Available, In Transit, At Port, Loading, Unloading)
        /// </summary>
        [Required]
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Current physical location of the container
        /// </summary>
        public string CurrentLocation { get; set; } = string.Empty;
        
        /// <summary>
        /// Final destination port or location
        /// </summary>
        public string Destination { get; set; } = string.Empty;
        
        /// <summary>
        /// Weight of the container in kilograms
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be positive")]
        public decimal Weight { get; set; }
        
        /// <summary>
        /// Maximum weight capacity in kilograms
        /// </summary>
        public decimal? MaxWeight { get; set; }
        
        /// <summary>
        /// Cargo description and contents
        /// </summary>
        public string CargoDescription { get; set; } = string.Empty;
        
        /// <summary>
        /// Container size (20ft, 40ft, 45ft)
        /// </summary>
        public string Size { get; set; } = string.Empty;
        
        /// <summary>
        /// Temperature setting for refrigerated containers (Celsius)
        /// </summary>
        public decimal? Temperature { get; set; }
        
        /// <summary>
        /// Container condition (Good, Damaged, Needs Repair)
        /// </summary>
        public string Condition { get; set; } = string.Empty;
        
        /// <summary>
        /// Last known GPS coordinates (latitude,longitude)
        /// </summary>
        public string Coordinates { get; set; } = string.Empty;
        
        /// <summary>
        /// Estimated time of arrival at destination
        /// </summary>
        public DateTime? EstimatedArrival { get; set; }
        
        /// <summary>
        /// Timestamp when the container was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Timestamp when the container was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Foreign key to the ship this container is currently on (nullable)
        /// </summary>
        public int? ShipId { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The ship this container is currently on
        /// </summary>
        [ForeignKey("ShipId")]
        public Ship? Ship { get; set; }
        
        /// <summary>
        /// Berth assignments for this container
        /// </summary>
        public ICollection<BerthAssignment> BerthAssignments { get; set; } = new List<BerthAssignment>();
        
        /// <summary>
        /// Ship container loading/unloading operations for this container
        /// </summary>
        public ICollection<ShipContainer> ShipContainers { get; set; } = new List<ShipContainer>();
        
        /// <summary>
        /// Movement tracking records for this container
        /// </summary>
        public ICollection<ContainerMovement> Movements { get; set; } = new List<ContainerMovement>();
        
        /// <summary>
        /// Events related to this container
        /// </summary>
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}