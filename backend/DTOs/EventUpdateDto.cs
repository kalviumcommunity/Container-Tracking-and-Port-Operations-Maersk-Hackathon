using System;

namespace Backend.DTOs
{
    /// <summary>
    /// Data Transfer Object for updating an event
    /// </summary>
    public class EventUpdateDto
    {
        /// <summary>
        /// Type of the event
        /// </summary>
        public string EventType { get; set; }
        
        /// <summary>
        /// Title/name of the event
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Detailed description of the event
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Priority/severity of the event (Critical, High, Medium, Low)
        /// </summary>
        public string Priority { get; set; }
        
        /// <summary>
        /// Status of the event (New, Acknowledged, In Progress, Resolved)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// User ID to whom the event is assigned
        /// </summary>
        public int? AssignedToUserId { get; set; }
    }
}
