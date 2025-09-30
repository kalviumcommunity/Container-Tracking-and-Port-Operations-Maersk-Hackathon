using System.Collections.Generic;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for Berth
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
        public string Name { get; set; }
        
        /// <summary>
        /// Maximum capacity of containers the berth can handle
        /// </summary>
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current status of the berth (e.g., Available, Occupied)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// Foreign key to the port this berth belongs to
        /// </summary>
        public int PortId { get; set; }
        
        /// <summary>
        /// The name of the port this berth belongs to
        /// </summary>
        public string PortName { get; set; }
        
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
        public string Name { get; set; }
        
        /// <summary>
        /// Maximum capacity of containers the berth can handle
        /// </summary>
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current status of the berth (e.g., Available, Occupied)
        /// </summary>
        public string Status { get; set; }
        
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
        public List<BerthAssignmentDto> BerthAssignments { get; set; }
    }
}