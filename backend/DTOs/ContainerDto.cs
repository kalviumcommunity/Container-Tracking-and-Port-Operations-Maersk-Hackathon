using System;
using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for Container
    /// </summary>
    public class ContainerDto
    {
        /// <summary>
        /// Unique identifier for the container using industry standard format
        /// </summary>
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
        
        /// <summary>
        /// Name of the ship this container is on (if any)
        /// </summary>
        public string ShipName { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a Container
    /// </summary>
    public class ContainerCreateUpdateDto
    {
        /// <summary>
        /// Unique identifier for the container using industry standard format
        /// </summary>
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
        /// Foreign key to the ship this container is currently on (nullable)
        /// </summary>
        public int? ShipId { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for Container with detailed information
    /// </summary>
    public class ContainerDetailDto : ContainerDto
    {
        /// <summary>
        /// The berth assignments for this container
        /// </summary>
        public List<BerthAssignmentDto> BerthAssignments { get; set; }
        
        /// <summary>
        /// The ship container loading operations for this container
        /// </summary>
        public List<ShipContainerDto> ShipContainers { get; set; }
    }
}