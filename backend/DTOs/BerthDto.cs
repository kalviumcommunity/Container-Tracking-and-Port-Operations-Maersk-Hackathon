using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for Berth - Enhanced with rich Model fields
    /// </summary>
    public class BerthDto
    {
        /// <summary>
        /// Unique identifier for the berth
        /// </summary>
        public int BerthId { get; set; }
        
        /// <summary>
        /// Name or identifier of the berth within the port
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Unique identifier code for the berth (e.g., "B001", "WHARF-A1")
        /// </summary>
        public string? Identifier { get; set; }
        
        /// <summary>
        /// Type of berth (e.g., Container, Bulk, RoRo, Cruise)
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// Maximum capacity of containers the berth can handle
        /// </summary>
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current load/occupancy of the berth
        /// </summary>
        public int CurrentLoad { get; set; }
        
        /// <summary>
        /// Maximum ship length that can dock at this berth (in meters)
        /// </summary>
        public decimal? MaxShipLength { get; set; }
        
        /// <summary>
        /// Maximum ship draft that can dock at this berth (in meters)
        /// </summary>
        public decimal? MaxDraft { get; set; }
        
        /// <summary>
        /// Current status of the berth (e.g., Available, Occupied, Maintenance)
        /// </summary>
        public required string Status { get; set; }
        
        /// <summary>
        /// Available services at this berth (comma-separated)
        /// </summary>
        public string? AvailableServices { get; set; }
        
        /// <summary>
        /// Number of cranes available at this berth
        /// </summary>
        public int? CraneCount { get; set; }
        
        /// <summary>
        /// Hourly rate for using this berth (in local currency)
        /// </summary>
        public decimal? HourlyRate { get; set; }
        
        /// <summary>
        /// Priority level of this berth (High, Medium, Low)
        /// </summary>
        public string? Priority { get; set; }
        
        /// <summary>
        /// Additional notes about the berth
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Foreign key to the port this berth belongs to
        /// </summary>
        public int PortId { get; set; }
        
        /// <summary>
        /// The name of the port this berth belongs to
        /// </summary>
        public required string PortName { get; set; }
        
        /// <summary>
        /// Number of active assignments
        /// </summary>
        public int ActiveAssignmentCount { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a Berth
    /// </summary>
    public class BerthCreateUpdateDto
    {
        /// <summary>
        /// Name or identifier of the berth within the port
        /// </summary>
        public required string Name { get; set; }
        
        /// <summary>
        /// Maximum capacity of containers the berth can handle
        /// </summary>
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current status of the berth (e.g., Available, Occupied)
        /// </summary>
        public required string Status { get; set; }
        
        /// <summary>
        /// Foreign key to the port this berth belongs to
        /// </summary>
        public int PortId { get; set; }
    }
    
    /// <summary>
    /// Data Transfer Object for Berth with detailed information
    /// </summary>
    public class BerthDetailDto : BerthDto
    {
        /// <summary>
        /// The assignments of containers to this berth
        /// </summary>
        public List<BerthAssignmentDto> BerthAssignments { get; set; } = new();
    }
}