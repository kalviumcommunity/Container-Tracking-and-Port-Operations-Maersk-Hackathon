namespace Backend.DTOs
{
    /// <summary>
    /// DTO for user login request
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username or email for login
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User's password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for user login response
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT token for authentication
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// User information
        /// </summary>
        public UserDto User { get; set; } = new UserDto();
    }

    /// <summary>
    /// DTO for user registration
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Username for the new user
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address for the new user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password for the new user
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the new user
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Phone number (optional)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Department or organization (optional)
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Assigned port ID (optional, for port-specific users)
        /// </summary>
        public int? PortId { get; set; }

        /// <summary>
        /// Initial roles to assign to the user
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for user information
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Assigned port information
        /// </summary>
        public PortDto? AssignedPort { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// User permissions
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// Account status
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Last login time
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Account creation time
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for updating user information
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Email address
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Department
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Assigned port ID
        /// </summary>
        public int? PortId { get; set; }

        /// <summary>
        /// Account status
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for changing password
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Current password
        /// </summary>
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New password
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for role information
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// Role ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Role description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Is system role
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Permissions associated with this role
        /// </summary>
        public List<string> Permissions { get; set; } = new List<string>();

        /// <summary>
        /// Number of users with this role
        /// </summary>
        public int UserCount { get; set; }
    }

    /// <summary>
    /// DTO for permission information
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// Permission ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Permission name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Permission description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Permission category
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Is system permission
        /// </summary>
        public bool IsSystemPermission { get; set; }
    }
}