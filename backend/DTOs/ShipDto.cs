using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for Ship
    /// </summary>
    public class ShipDto
    {
        /// <summary>
        /// Unique identifier for the ship
        /// </summary>
        public int ShipId { get; set; }
        
        /// <summary>
        /// Name of the ship
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Current operational status of the ship (e.g., At Sea, Docked)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// The number of containers currently on this ship
        /// </summary>
        public int ContainerCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a Ship
    /// </summary>
    public class ShipCreateUpdateDto
    {
        /// <summary>
        /// Name of the ship
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Current operational status of the ship (e.g., At Sea, Docked)
        /// </summary>
        public string Status { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for Ship with detailed information
    /// </summary>
    public class ShipDetailDto : ShipDto
    {
        /// <summary>
        /// The containers currently on this ship
        /// </summary>
        public List<ContainerDto> Containers { get; set; }
        
        /// <summary>
        /// The container loading operations for this ship
        /// </summary>
        public List<ShipContainerDto> ShipContainers { get; set; }
    }
}