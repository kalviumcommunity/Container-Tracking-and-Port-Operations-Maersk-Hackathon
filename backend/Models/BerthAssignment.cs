using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents an assignment of a container to a berth for a specific time period
    /// </summary>
    public class BerthAssignment
    {
        /// <summary>
        /// Unique identifier for the berth assignment
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key to the container assigned to the berth
        /// </summary>
        [Required]
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Foreign key to the berth the container is assigned to
        /// </summary>
        [Required]
        public int BerthId { get; set; }
        
        /// <summary>
        /// Time when the container was assigned to the berth
        /// </summary>
        [Required]
        public DateTime AssignedAt { get; set; }
        
        /// <summary>
        /// Time when the container was released from the berth (null if still assigned)
        /// </summary>
        public DateTime? ReleasedAt { get; set; }
        
        // Navigation properties
        /// <summary>
        /// The container assigned to the berth
        /// </summary>
        [ForeignKey("ContainerId")]
        public Container Container { get; set; }
        
        /// <summary>
        /// The berth the container is assigned to
        /// </summary>
        [ForeignKey("BerthId")]
        public Berth Berth { get; set; }
    }
}