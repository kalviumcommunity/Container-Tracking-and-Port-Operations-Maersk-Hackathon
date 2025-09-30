using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    /// <summary>
    /// Represents a user in the system with authentication and role information
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Username for login (unique)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User's email address (unique)
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password for authentication
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's full name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User's phone number
        /// </summary>
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// User's department or organization
        /// </summary>
        [StringLength(100)]
        public string? Department { get; set; }

        /// <summary>
        /// Specific port access (null means access to all ports)
        /// </summary>
        public int? PortId { get; set; }

        /// <summary>
        /// Navigation property to assigned port
        /// </summary>
        public Port? Port { get; set; }

        /// <summary>
        /// Indicates if the user account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Date when the user account was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the user account was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last login timestamp
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// User roles for authorization
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        
    }
}