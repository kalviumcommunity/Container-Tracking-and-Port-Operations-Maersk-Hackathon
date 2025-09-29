using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents a container loading or unloading operation for a ship
    /// </summary>
    public class ShipContainer
    {
        /// <summary>
        /// Unique identifier for the ship container operation
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key to the ship involved in the operation
        /// </summary>
        [Required]
        public int ShipId { get; set; }
        
        /// <summary>
        /// Foreign key to the container being loaded/unloaded
        /// </summary>
        [Required]
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Time when the container was loaded onto the ship
        /// </summary>
        [Required]
        public DateTime LoadedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The ship involved in the operation
        /// </summary>
        [ForeignKey("ShipId")]
        public Ship Ship { get; set; }
        
        /// <summary>
        /// The container being loaded/unloaded
        /// </summary>
        [ForeignKey("ContainerId")]
        public Container Container { get; set; }
    }
}