namespace Backend.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between Users and Roles
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Foreign key to User
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Navigation property to User
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Foreign key to Role
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Navigation property to Role
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Date when the role was assigned to the user
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who assigned this role (optional)
        /// </summary>
        public int? AssignedByUserId { get; set; }

        /// <summary>
        /// Navigation property to the user who assigned this role
        /// </summary>
        public User? AssignedByUser { get; set; }

        /// <summary>
        /// Indicates if this role assignment is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}