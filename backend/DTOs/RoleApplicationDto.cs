using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    /// <summary>
    /// DTO for submitting a role application
    /// </summary>
    public class RoleApplicationRequestDto
    {
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
    }
    
    /// <summary>
    /// DTO for role application response
    /// </summary>
    public class RoleApplicationDto
    {
        /// <summary>
        /// Unique identifier for the role application
        /// </summary>
        public int ApplicationId { get; set; }
        
        /// <summary>
        /// User ID who submitted the application
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Username of the applicant
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Full name of the applicant
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email of the applicant
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Role being requested
        /// </summary>
        public string RequestedRole { get; set; } = string.Empty;
        
        /// <summary>
        /// User's justification for requesting this role
        /// </summary>
        public string Justification { get; set; } = string.Empty;
        
        /// <summary>
        /// Application status (Pending, Approved, Rejected)
        /// </summary>
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// When the application was submitted
        /// </summary>
        public DateTime RequestedAt { get; set; }
        
        /// <summary>
        /// Admin user ID who reviewed the application
        /// </summary>
        public int? ReviewedBy { get; set; }
        
        /// <summary>
        /// Username of the admin who reviewed
        /// </summary>
        public string? ReviewedByUsername { get; set; }
        
        /// <summary>
        /// Full name of the admin who reviewed
        /// </summary>
        public string? ReviewedByFullName { get; set; }
        
        /// <summary>
        /// When the application was reviewed
        /// </summary>
        public DateTime? ReviewedAt { get; set; }
        
        /// <summary>
        /// Admin's notes/comments on the review
        /// </summary>
        public string? ReviewNotes { get; set; }
    }
    
    /// <summary>
    /// DTO for reviewing role applications
    /// </summary>
    public class ReviewApplicationDto
    {
        /// <summary>
        /// Review decision (Approved or Rejected)
        /// </summary>
        [Required]
        public string Status { get; set; } = string.Empty; // "Approved" or "Rejected"
        
        /// <summary>
        /// Admin's notes/comments on the review
        /// </summary>
        [MaxLength(500)]
        public string? ReviewNotes { get; set; }
    }
    
    /// <summary>
    /// Available roles that can be requested
    /// </summary>
    public class AvailableRoleDto
    {
        /// <summary>
        /// Role name
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
        
        /// <summary>
        /// Description of what this role can do
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether user can apply for this role
        /// </summary>
        public bool CanApply { get; set; }
        
        /// <summary>
        /// Reason why user cannot apply (if CanApply is false)
        /// </summary>
        public string? ReasonCannotApply { get; set; }
    }
}