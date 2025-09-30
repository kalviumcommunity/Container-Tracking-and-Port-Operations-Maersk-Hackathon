namespace Backend.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between Roles and Permissions
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// Foreign key to Role
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Navigation property to Role
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Foreign key to Permission
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Navigation property to Permission
        /// </summary>
        public Permission Permission { get; set; } = null!;

        /// <summary>
        /// Date when the permission was granted to the role
        /// </summary>
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who granted this permission (optional)
        /// </summary>
        public int? GrantedByUserId { get; set; }

        /// <summary>
        /// Navigation property to the user who granted this permission
        /// </summary>
        public User? GrantedByUser { get; set; }
    }
}