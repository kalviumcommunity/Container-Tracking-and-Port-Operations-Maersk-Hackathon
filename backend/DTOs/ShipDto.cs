using System;
using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for Ship - Enhanced with rich Model fields
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
        public required string Name { get; set; }
        
        /// <summary>
        /// International Maritime Organization number (unique ship identifier)
        /// </summary>
        public string? ImoNumber { get; set; }
        
        /// <summary>
        /// Flag state of the ship (country of registration)
        /// </summary>
        public string? Flag { get; set; }
        
        /// <summary>
        /// Type of ship (e.g., Container Ship, Bulk Carrier, Tanker)
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// Container capacity of the ship (TEU - Twenty-foot Equivalent Unit)
        /// </summary>
        public int? Capacity { get; set; }
        
        /// <summary>
        /// Current operational status of the ship (e.g., At Sea, Docked, Loading)
        /// </summary>
        public required string Status { get; set; }
        
        /// <summary>
        /// Length of the ship in meters
        /// </summary>
        public decimal? Length { get; set; }
        
        /// <summary>
        /// Beam (width) of the ship in meters
        /// </summary>
        public decimal? Beam { get; set; }
        
        /// <summary>
        /// Draft (depth below waterline) of the ship in meters
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
        /// Current GPS coordinates of the ship (latitude, longitude)
        /// </summary>
        public string? Coordinates { get; set; }
        
        /// <summary>
        /// Current speed of the ship in knots
        /// </summary>
        public decimal? Speed { get; set; }
        
        /// <summary>
        /// Current heading/direction of the ship in degrees
        /// </summary>
        public decimal? Heading { get; set; }
        
        /// <summary>
        /// Next port destination
        /// </summary>
        public string? NextPort { get; set; }
        
        /// <summary>
        /// Estimated time of arrival at next port
        /// </summary>
        public DateTime? EstimatedArrival { get; set; }
        
        /// <summary>
        /// Current port ID if docked
        /// </summary>
        public int? CurrentPortId { get; set; }
        
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
        public required string Name { get; set; }
        
        /// <summary>
        /// Current operational status of the ship (e.g., At Sea, Docked)
        /// </summary>
        public required string Status { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for Ship with detailed information
    /// </summary>
    public class ShipDetailDto : ShipDto
    {
        /// <summary>
        /// The containers currently on this ship
        /// </summary>
        public List<ContainerDto> Containers { get; set; } = new();
        
        /// <summary>
        /// The container loading operations for this ship
        /// </summary>
        public List<ShipContainerDto> ShipContainers { get; set; } = new();
    }
}