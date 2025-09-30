using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    /// <summary>
    /// Represents a role in the system with specific permissions
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Unique identifier for the role
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Role name (e.g., Admin, Operator, Viewer, PortManager)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the role and its permissions
        /// </summary>
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the role is a system role that cannot be deleted
        /// </summary>
        public bool IsSystemRole { get; set; } = false;

        /// <summary>
        /// Date when the role was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Users assigned to this role
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Permissions associated with this role
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}