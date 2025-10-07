using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    /// <summary>
    /// DTO for user list in admin dashboard
    /// </summary>
    public class UserListDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// DTO for updating user roles
    /// </summary>
    public class UpdateUserRolesDto
    {
        [Required]
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// DTO for user status update
    /// </summary>
    public class UpdateUserStatusDto
    {
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
    }

    /// <summary>
    /// DTO for block/unblock user request
    /// </summary>
    public class BlockUserDto
    {
        public bool IsBlocked { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// DTO for system statistics
    /// </summary>
    public class SystemStatsDto
    {
        public int Users { get; set; }
        public int Containers { get; set; }
        public int Ships { get; set; }
        public int Berths { get; set; }
        public int Ports { get; set; }
        public int ActiveUsers { get; set; }
        public int PendingRoleApplications { get; set; }
        public int BlockedUsers { get; set; }
        public int DeletedUsers { get; set; }
    }

    /// <summary>
    /// Response wrapper for paginated user lists
    /// </summary>
    public class UsersPagedResponse
    {
        public List<UserListDto> Users { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}