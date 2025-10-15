using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for Port
    /// </summary>
    public class PortDto
    {
        /// <summary>
        /// Unique identifier for the port
        /// </summary>
        public int PortId { get; set; }
        
        /// <summary>
        /// Name of the port
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// International port code (e.g., USNYC, USLA)
        /// </summary>
        public string? Code { get; set; }
        
        /// <summary>
        /// Country where the port is located
        /// </summary>
        public string? Country { get; set; }
        
        /// <summary>
        /// Geographic location of the port
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// GPS coordinates (latitude,longitude)
        /// </summary>
        public string? Coordinates { get; set; }
        
        /// <summary>
        /// Maximum number of containers the port can handle
        /// </summary>
        public int TotalContainerCapacity { get; set; }
        
        /// <summary>
        /// Current number of containers at the port
        /// </summary>
        public int CurrentContainerCount { get; set; }
        
        /// <summary>
        /// Maximum number of ships the port can accommodate
        /// </summary>
        public int MaxShipCapacity { get; set; }
        
        /// <summary>
        /// Current number of ships at the port
        /// </summary>
        public int CurrentShipCount { get; set; }
        
        /// <summary>
        /// Operating hours (e.g., "24/7", "06:00-22:00")
        /// </summary>
        public string? OperatingHours { get; set; }
        
        /// <summary>
        /// Time zone (e.g., "UTC+5", "EST")
        /// </summary>
        public string? TimeZone { get; set; }
        
        /// <summary>
        /// Contact information
        /// </summary>
        public string? ContactInfo { get; set; }
        
        /// <summary>
        /// Available services (Container, Bulk, Tanker, Passenger)
        /// </summary>
        public string? Services { get; set; }
        
        /// <summary>
        /// Current operational status (Active, Maintenance, Closed)
        /// </summary>
        public string? Status { get; set; }
        
        /// <summary>
        /// The number of berths within this port
        /// </summary>
        public int BerthCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a Port
    /// </summary>
    public class PortCreateUpdateDto
    {
        /// <summary>
        /// Name of the port
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// International port code (e.g., USNYC, USLA)
        /// </summary>
        public string? Code { get; set; }
        
        /// <summary>
        /// Country where the port is located
        /// </summary>
        public string? Country { get; set; }
        
        /// <summary>
        /// Geographic location of the port
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// GPS coordinates (latitude,longitude)
        /// </summary>
        public string? Coordinates { get; set; }
        
        /// <summary>
        /// Maximum number of containers the port can handle
        /// </summary>
        public int TotalContainerCapacity { get; set; }
        
        /// <summary>
        /// Maximum number of ships the port can accommodate
        /// </summary>
        public int MaxShipCapacity { get; set; }
        
        /// <summary>
        /// Operating hours (e.g., "24/7", "06:00-22:00")
        /// </summary>
        public string? OperatingHours { get; set; }
        
        /// <summary>
        /// Time zone (e.g., "UTC+5", "EST")
        /// </summary>
        public string? TimeZone { get; set; }
        
        /// <summary>
        /// Contact information
        /// </summary>
        public string? ContactInfo { get; set; }
        
        /// <summary>
        /// Available services (Container, Bulk, Tanker, Passenger)
        /// </summary>
        public string? Services { get; set; }
        
        /// <summary>
        /// Current operational status (Active, Maintenance, Closed)
        /// </summary>
        public string? Status { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for Port with detailed information
    /// </summary>
    public class PortDetailDto : PortDto
    {
        /// <summary>
        /// The berths within this port
        /// </summary>
        public List<BerthDto> Berths { get; set; }
    }
}