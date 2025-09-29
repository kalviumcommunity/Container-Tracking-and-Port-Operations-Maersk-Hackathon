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
        /// Unique identifier for the container using industry standard format
        /// </summary>
        [Key]
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Name or description of the container
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Type of container (dry, refrigerated, liquid, hazardous, etc.)
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Current status of the container (e.g., Empty, Loaded, In Transit)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Current physical location of the container
        /// </summary>
        public string CurrentLocation { get; set; }
        
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
        public Ship Ship { get; set; }
        
        /// <summary>
        /// Berth assignments for this container
        /// </summary>
        public ICollection<BerthAssignment> BerthAssignments { get; set; }
        
        /// <summary>
        /// Ship container loading/unloading operations for this container
        /// </summary>
        public ICollection<ShipContainer> ShipContainers { get; set; }
    }
}