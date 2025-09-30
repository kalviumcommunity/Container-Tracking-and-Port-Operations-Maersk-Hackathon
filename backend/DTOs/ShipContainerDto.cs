using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for ShipContainer
    /// </summary>
    public class ShipContainerDto
    {
        /// <summary>
        /// Unique identifier for the ship container operation
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key to the ship involved in the operation
        /// </summary>
        public int ShipId { get; set; }
        
        /// <summary>
        /// The name of the ship involved in the operation
        /// </summary>
        public string ShipName { get; set; }
        
        /// <summary>
        /// Foreign key to the container being loaded/unloaded
        /// </summary>
        public string ContainerId { get; set; }
        
        /// <summary>
        /// The name of the container being loaded/unloaded
        /// </summary>
        public string ContainerName { get; set; }
        
        /// <summary>
        /// Time when the container was loaded onto the ship
        /// </summary>
        public DateTime LoadedAt { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a ShipContainer
    /// </summary>
    public class ShipContainerCreateUpdateDto
    {
        /// <summary>
        /// Foreign key to the ship involved in the operation
        /// </summary>
        public int ShipId { get; set; }
        
        /// <summary>
        /// Foreign key to the container being loaded/unloaded
        /// </summary>
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Time when the container was loaded onto the ship
        /// </summary>
        public DateTime? LoadedAt { get; set; }
    }
}