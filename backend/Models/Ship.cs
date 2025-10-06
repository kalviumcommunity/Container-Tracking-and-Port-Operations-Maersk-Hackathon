using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents a cargo ship that transports containers
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// Unique identifier for the ship
        /// </summary>
        [Key]
        public int ShipId { get; set; }
        
        /// <summary>
        /// Name of the ship
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// IMO (International Maritime Organization) number
        /// </summary>
        [MaxLength(20)]
        public string ImoNumber { get; set; }
        
        /// <summary>
        /// Ship's flag state/country of registration
        /// </summary>
        [MaxLength(50)]
        public string Flag { get; set; }
        
        /// <summary>
        /// Type of ship (Container Ship, Bulk Carrier, Tanker, etc.)
        /// </summary>
        [MaxLength(50)]
        public string Type { get; set; }
        
        /// <summary>
        /// Container capacity in TEU (Twenty-foot Equivalent Units)
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be positive")]
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current operational status of the ship (At Sea, Docked, Loading, Unloading, In Transit)
        /// </summary>
        [Required]
        public string Status { get; set; }
        
        /// <summary>
        /// Overall length of the ship in meters
        /// </summary>
        public decimal? Length { get; set; }
        
        /// <summary>
        /// Beam (width) of the ship in meters
        /// </summary>
        public decimal? Beam { get; set; }
        
        /// <summary>
        /// Draft (depth underwater) in meters
        /// </summary>
        public decimal? Draft { get; set; }
        
        /// <summary>
        /// Gross tonnage of the ship
        /// </summary>
        public decimal? GrossTonnage { get; set; }
        
        /// <summary>
        /// Year the ship was built
        /// </summary>
        public int? YearBuilt { get; set; }
        
        /// <summary>
        /// Current GPS coordinates (latitude,longitude)
        /// </summary>
        public string Coordinates { get; set; }
        
        /// <summary>
        /// Current speed in knots
        /// </summary>
        public decimal? Speed { get; set; }
        
        /// <summary>
        /// Current heading in degrees (0-360)
        /// </summary>
        public decimal? Heading { get; set; }
        
        /// <summary>
        /// Next port of call
        /// </summary>
        public string NextPort { get; set; }
        
        /// <summary>
        /// Estimated time of arrival at next port
        /// </summary>
        public DateTime? EstimatedArrival { get; set; }
        
        /// <summary>
        /// Foreign key to the current port (nullable)
        /// </summary>
        public int? CurrentPortId { get; set; }
        
        /// <summary>
        /// Timestamp when the ship record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Timestamp when the ship record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The current port where the ship is docked (if any)
        /// </summary>
        [ForeignKey("CurrentPortId")]
        public Port CurrentPort { get; set; }
        
        /// <summary>
        /// The containers currently on this ship
        /// </summary>
        public ICollection<Container> Containers { get; set; }
        
        /// <summary>
        /// The container loading operations for this ship
        /// </summary>
        public ICollection<ShipContainer> ShipContainers { get; set; }
        
        /// <summary>
        /// Berth assignments for this ship
        /// </summary>
        public ICollection<BerthAssignment> BerthAssignments { get; set; }
    }
}