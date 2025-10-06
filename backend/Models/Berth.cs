using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents a berth within a port where ships can dock
    /// </summary>
    public class Berth
    {
        /// <summary>
        /// Unique identifier for the berth
        /// </summary>
        [Key]
        public int BerthId { get; set; }
        
        /// <summary>
        /// Name or identifier of the berth within the port
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        /// <summary>
        /// Berth number or identifier (e.g., B-001, A-12)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Identifier { get; set; }
        
        /// <summary>
        /// Type of berth (Container, Bulk, Tanker, RoRo, Multipurpose)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        
        /// <summary>
        /// Current status of the berth (Available, Occupied, Maintenance, Reserved)
        /// </summary>
        [Required]
        public string Status { get; set; }
        
        /// <summary>
        /// Maximum capacity of containers the berth can handle
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current number of containers at this berth
        /// </summary>
        public int CurrentLoad { get; set; }
        
        /// <summary>
        /// Maximum length of ship that can dock (meters)
        /// </summary>
        public decimal? MaxShipLength { get; set; }
        
        /// <summary>
        /// Maximum draft (depth) for ships (meters)
        /// </summary>
        public decimal? MaxDraft { get; set; }
        
        /// <summary>
        /// Available services at this berth (Crane, Refueling, Maintenance)
        /// </summary>
        public string AvailableServices { get; set; }
        
        /// <summary>
        /// Number of cranes available at this berth
        /// </summary>
        public int? CraneCount { get; set; }
        
        /// <summary>
        /// Hourly rate for using this berth
        /// </summary>
        public decimal? HourlyRate { get; set; }
        
        /// <summary>
        /// Priority level for berth assignments (High, Medium, Low)
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Notes or special instructions for this berth
        /// </summary>
        public string Notes { get; set; }
        
        /// <summary>
        /// Timestamp when the berth was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Timestamp when the berth was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Foreign key to the port this berth belongs to
        /// </summary>
        [Required]
        public int PortId { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The port this berth belongs to
        /// </summary>
        [ForeignKey("PortId")]
        public Port Port { get; set; }
        
        /// <summary>
        /// The assignments of ships/containers to this berth
        /// </summary>
        public ICollection<BerthAssignment> BerthAssignments { get; set; }
        
        /// <summary>
        /// Container movements involving this berth
        /// </summary>
        public ICollection<ContainerMovement> ContainerMovements { get; set; }
        
        /// <summary>
        /// Events related to this berth
        /// </summary>
        public ICollection<Event> Events { get; set; }
        
        /// <summary>
        /// Analytics data for this berth
        /// </summary>
        public ICollection<Analytics> Analytics { get; set; }
    }
}