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
        public string Name { get; set; }
        
        /// <summary>
        /// Maximum capacity of containers the berth can handle
        /// </summary>
        [Required]
        public int Capacity { get; set; }
        
        /// <summary>
        /// Current status of the berth (e.g., Available, Occupied)
        /// </summary>
        [Required]
        public string Status { get; set; }
        
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
        /// The assignments of containers to this berth
        /// </summary>
        public ICollection<BerthAssignment> BerthAssignments { get; set; }
    }
}