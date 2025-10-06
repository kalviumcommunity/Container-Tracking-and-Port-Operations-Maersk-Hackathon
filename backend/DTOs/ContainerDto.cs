using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        /// Final destination port or location
        /// </summary>
        public string Destination { get; set; }
        
        /// <summary>
        /// Weight of the container in kilograms
        /// </summary>
        public decimal Weight { get; set; }
        
        /// <summary>
        /// Maximum weight capacity in kilograms
        /// </summary>
        public decimal? MaxWeight { get; set; }
        
        /// <summary>
        /// Cargo description and contents
        /// </summary>
        public string CargoDescription { get; set; }
        
        /// <summary>
        /// Container size (20ft, 40ft, 45ft)
        /// </summary>
        public string Size { get; set; }
        
        /// <summary>
        /// Temperature setting for refrigerated containers (Celsius)
        /// </summary>
        public decimal? Temperature { get; set; }
        
        /// <summary>
        /// Container condition (Good, Damaged, Needs Repair)
        /// </summary>
        public string Condition { get; set; }
        
        /// <summary>
        /// Last known GPS coordinates (latitude,longitude)
        /// </summary>
        public string Coordinates { get; set; }
        
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
        
        /// <summary>
        /// Name of the ship this container is on (if any)
        /// </summary>
        public string ShipName { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for creating a Container
    /// </summary>
    public class ContainerCreateDto
    {
        /// <summary>
        /// Unique identifier for the container using industry standard format
        /// </summary>
        [Required]
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Name or description of the container
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Type of container (dry, refrigerated, liquid, hazardous, etc.)
        /// </summary>
        [Required]
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
    /// Data Transfer Object for updating a Container
    /// </summary>
    public class ContainerUpdateDto
    {
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
    /// Data Transfer Object for creating or updating a Container (legacy - use ContainerCreateDto or ContainerUpdateDto)
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