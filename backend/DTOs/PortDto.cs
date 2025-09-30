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
        /// Geographic location of the port
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Maximum number of containers the port can handle
        /// </summary>
        public int TotalContainerCapacity { get; set; }
        
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
        /// Geographic location of the port
        /// </summary>
        public string Location { get; set; }
        
        /// <summary>
        /// Maximum number of containers the port can handle
        /// </summary>
        public int TotalContainerCapacity { get; set; }
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