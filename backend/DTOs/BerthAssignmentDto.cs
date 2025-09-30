using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for BerthAssignment
    /// </summary>
    public class BerthAssignmentDto
    {
        /// <summary>
        /// Unique identifier for the berth assignment
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Foreign key to the container assigned to the berth
        /// </summary>
        public string ContainerId { get; set; }
        
        /// <summary>
        /// The name of the container assigned to the berth
        /// </summary>
        public string ContainerName { get; set; }
        
        /// <summary>
        /// Foreign key to the berth the container is assigned to
        /// </summary>
        public int BerthId { get; set; }
        
        /// <summary>
        /// The name of the berth the container is assigned to
        /// </summary>
        public string BerthName { get; set; }
        
        /// <summary>
        /// Time when the container was assigned to the berth
        /// </summary>
        public DateTime AssignedAt { get; set; }
        
        /// <summary>
        /// Time when the container was released from the berth (null if still assigned)
        /// </summary>
        public DateTime? ReleasedAt { get; set; }
        
        /// <summary>
        /// The status of the assignment (Active or Released)
        /// </summary>
        public string Status => ReleasedAt.HasValue ? "Released" : "Active";
    }
    
    /// <summary>
    /// Data Transfer Object for creating or updating a BerthAssignment
    /// </summary>
    public class BerthAssignmentCreateUpdateDto
    {
        /// <summary>
        /// Foreign key to the container assigned to the berth
        /// </summary>
        public string ContainerId { get; set; }
        
        /// <summary>
        /// Foreign key to the berth the container is assigned to
        /// </summary>
        public int BerthId { get; set; }
        
        /// <summary>
        /// Time when the container was assigned to the berth
        /// </summary>
        public DateTime? AssignedAt { get; set; }
        
        /// <summary>
        /// Time when the container was released from the berth (null if still assigned)
        /// </summary>
        public DateTime? ReleasedAt { get; set; }
    }
}