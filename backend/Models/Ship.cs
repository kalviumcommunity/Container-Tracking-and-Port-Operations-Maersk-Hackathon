using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public string Name { get; set; }
        
        /// <summary>
        /// Current operational status of the ship (e.g., At Sea, Docked)
        /// </summary>
        [Required]
        public string Status { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The containers currently on this ship
        /// </summary>
        public ICollection<Container> Containers { get; set; }
        
        /// <summary>
        /// The container loading operations for this ship
        /// </summary>
        public ICollection<ShipContainer> ShipContainers { get; set; }
    }
}