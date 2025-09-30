using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    /// <summary>
    /// Represents a specific permission in the system
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Unique identifier for the permission
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Permission name (e.g., "containers.read", "containers.write", "containers.delete")
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of what this permission allows
        /// </summary>
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of the permission (e.g., "Containers", "Ships", "Ports")
        /// </summary>
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this is a system permission that cannot be deleted
        /// </summary>
        public bool IsSystemPermission { get; set; } = false;

        /// <summary>
        /// Date when the permission was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Roles that have this permission
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}