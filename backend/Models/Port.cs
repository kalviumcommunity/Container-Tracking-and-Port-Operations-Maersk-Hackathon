using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    /// <summary>
    /// Represents a shipping port with berths and container capacity
    /// </summary>
    public class Port
    {
        /// <summary>
        /// Unique identifier for the port
        /// </summary>
        [Key]
        public int PortId { get; set; }
        
        /// <summary>
        /// Name of the port
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// International port code (e.g., USNYC, USLA)
        /// </summary>
        [MaxLength(10)]
        public string Code { get; set; }
        
        /// <summary>
        /// Country where the port is located
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Country { get; set; }
        
        /// <summary>
        /// Geographic location/city of the port
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Location { get; set; }
        
        /// <summary>
        /// GPS coordinates (latitude,longitude)
        /// </summary>
        public string Coordinates { get; set; }
        
        /// <summary>
        /// Maximum number of containers the port can handle
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int TotalContainerCapacity { get; set; }
        
        /// <summary>
        /// Current number of containers at the port
        /// </summary>
        public int CurrentContainerCount { get; set; }
        
        /// <summary>
        /// Maximum number of ships that can dock simultaneously
        /// </summary>
        public int MaxShipCapacity { get; set; }
        
        /// <summary>
        /// Current number of docked ships
        /// </summary>
        public int CurrentShipCount { get; set; }
        
        /// <summary>
        /// Operating hours (e.g., "24/7", "06:00-22:00")
        /// </summary>
        public string OperatingHours { get; set; }
        
        /// <summary>
        /// Port timezone (e.g., "UTC+5", "EST")
        /// </summary>
        public string TimeZone { get; set; }
        
        /// <summary>
        /// Contact information for port operations
        /// </summary>
        public string ContactInfo { get; set; }
        
        /// <summary>
        /// Port services available (Container, Bulk, Tanker, Passenger)
        /// </summary>
        public string Services { get; set; }
        
        /// <summary>
        /// Current operational status (Active, Maintenance, Closed)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Timestamp when the port record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Timestamp when the port record was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The berths within this port
        /// </summary>
        public ICollection<Berth> Berths { get; set; }
        
        /// <summary>
        /// Users assigned to manage this specific port
        /// </summary>
        public ICollection<User> AssignedUsers { get; set; } = new List<User>();
        
        /// <summary>
        /// Ships currently at this port
        /// </summary>
        public ICollection<Ship> DockedShips { get; set; }
    }
}