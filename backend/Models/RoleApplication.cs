using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents a user's application for additional roles/permissions
    /// </summary>
    public class RoleApplication
    {
        /// <summary>
        /// Unique identifier for the role application
        /// </summary>
        [Key]
        public int ApplicationId { get; set; }
        
        /// <summary>
        /// User ID who is applying for the role
        /// </summary>
        [Required]
        public int UserId { get; set; }
        
        /// <summary>
        /// Role being requested (PortManager, Operator, etc.)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RequestedRole { get; set; } = string.Empty;
        
        /// <summary>
        /// User's justification for requesting this role
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Justification { get; set; } = string.Empty;
        
        /// <summary>
        /// Application status (Pending, Approved, Rejected)
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";
        
        /// <summary>
        /// When the application was submitted
        /// </summary>
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Admin user ID who reviewed the application
        /// </summary>
        public int? ReviewedBy { get; set; }
        
        /// <summary>
        /// When the application was reviewed
        /// </summary>
        public DateTime? ReviewedAt { get; set; }
        
        /// <summary>
        /// Admin's notes/comments on the review
        /// </summary>
        [MaxLength(500)]
        public string? ReviewNotes { get; set; }
        
        // Navigation properties
        /// <summary>
        /// User who submitted the application
        /// </summary>
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        
        /// <summary>
        /// Admin user who reviewed the application
        /// </summary>
        [ForeignKey("ReviewedBy")]
        public User? ReviewedByUser { get; set; }
    }
}