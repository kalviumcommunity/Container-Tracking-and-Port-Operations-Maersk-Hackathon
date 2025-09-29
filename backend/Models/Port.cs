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
        public string Name { get; set; }
        
        /// <summary>
        /// Geographic location of the port
        /// </summary>
        [Required]
        public string Location { get; set; }
        
        /// <summary>
        /// Maximum number of containers the port can handle
        /// </summary>
        [Required]
        public int TotalContainerCapacity { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The berths within this port
        /// </summary>
        public ICollection<Berth> Berths { get; set; }
    }
}