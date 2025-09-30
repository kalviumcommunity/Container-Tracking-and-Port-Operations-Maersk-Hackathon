using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;

namespace Backend.Services
{
    /// <summary>
    /// Interface for authentication service
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticate user with username/email and password
        /// </summary>
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Register a new user
        /// </summary>
        Task<UserDto> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Get user by ID with roles and permissions
        /// </summary>
        Task<UserDto?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Update user information
        /// </summary>
        Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateDto);

        /// <summary>
        /// Change user password
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Assign roles to user
        /// </summary>
        Task<bool> AssignRolesToUserAsync(int userId, List<string> roleNames);

        /// <summary>
        /// Remove roles from user
        /// </summary>
        Task<bool> RemoveRolesFromUserAsync(int userId, List<string> roleNames);

        /// <summary>
        /// Get user roles and permissions
        /// </summary>
        Task<(List<string> roles, List<string> permissions)> GetUserRolesAndPermissionsAsync(int userId);

        /// <summary>
        /// Deactivate user account
        /// </summary>
        Task<bool> DeactivateUserAsync(int userId);

        /// <summary>
        /// Activate user account
        /// </summary>
        Task<bool> ActivateUserAsync(int userId);
    }

    /// <summary>
    /// Authentication service implementation
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Authenticate user and return JWT token
        /// </summary>
        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Find user by username or email
            var user = await _context.Users
                .Include(u => u.Port)
                .FirstOrDefaultAsync(u => 
                    (u.Username == loginDto.Username || u.Email == loginDto.Username) 
                    && u.IsActive);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Get user roles and permissions
            var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user.UserId);

            // Generate JWT token
            var token = _jwtService.GenerateToken(user, roles, permissions);
            var expires = _jwtService.GetTokenExpiration();

            return new LoginResponseDto
            {
                Token = token,
                Expires = expires,
                User = MapToUserDto(user, roles, permissions)
            };
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username or email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Username or email already exists");
            }

            // Validate port if specified
            if (registerDto.PortId.HasValue)
            {
                var portExists = await _context.Ports.AnyAsync(p => p.PortId == registerDto.PortId.Value);
                if (!portExists)
                {
                    throw new InvalidOperationException("Invalid port ID");
                }
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                Department = registerDto.Department,
                PortId = registerDto.PortId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Assign roles if specified
            if (registerDto.Roles.Any())
            {
                await AssignRolesToUserAsync(user.UserId, registerDto.Roles);
            }

            // Get assigned roles and permissions
            var (roles, permissions) = await GetUserRolesAndPermissionsAsync(user.UserId);

            return MapToUserDto(user, roles, permissions);
        }

        /// <summary>
        /// Get user by ID with roles and permissions
        /// </summary>
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Port)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            var (roles, permissions) = await GetUserRolesAndPermissionsAsync(userId);
            return MapToUserDto(user, roles, permissions);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateDto)
        {
            var user = await _context.Users
                .Include(u => u.Port)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            // Update fields if provided
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                // Check if email is already in use by another user
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == updateDto.Email && u.UserId != userId);
                if (emailExists)
                {
                    throw new InvalidOperationException("Email already in use");
                }
                user.Email = updateDto.Email;
            }

            if (!string.IsNullOrEmpty(updateDto.FullName))
                user.FullName = updateDto.FullName;

            if (updateDto.PhoneNumber != null)
                user.PhoneNumber = updateDto.PhoneNumber;

            if (updateDto.Department != null)
                user.Department = updateDto.Department;

            if (updateDto.PortId.HasValue)
            {
                var portExists = await _context.Ports.AnyAsync(p => p.PortId == updateDto.PortId.Value);
                if (!portExists)
                {
                    throw new InvalidOperationException("Invalid port ID");
                }
                user.PortId = updateDto.PortId;
            }

            if (updateDto.IsActive.HasValue)
                user.IsActive = updateDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var (roles, permissions) = await GetUserRolesAndPermissionsAsync(userId);
            return MapToUserDto(user, roles, permissions);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Verify current password
            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            // Update password
            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Assign roles to user
        /// </summary>
        public async Task<bool> AssignRolesToUserAsync(int userId, List<string> roleNames)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            var roles = await _context.Roles
                .Where(r => roleNames.Contains(r.Name))
                .ToListAsync();

            if (roles.Count != roleNames.Count)
                return false; // Some roles don't exist

            // Get existing user roles
            var existingUserRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            // Add new roles that user doesn't already have
            foreach (var role in roles)
            {
                if (!existingUserRoles.Any(ur => ur.RoleId == role.RoleId))
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = role.RoleId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Remove roles from user
        /// </summary>
        public async Task<bool> RemoveRolesFromUserAsync(int userId, List<string> roleNames)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            var roles = await _context.Roles
                .Where(r => roleNames.Contains(r.Name))
                .ToListAsync();

            var userRolesToRemove = await _context.UserRoles
                .Where(ur => ur.UserId == userId && roles.Select(r => r.RoleId).Contains(ur.RoleId))
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRolesToRemove);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Get user roles and permissions
        /// </summary>
        public async Task<(List<string> roles, List<string> permissions)> GetUserRolesAndPermissionsAsync(int userId)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            var roles = userRoles.Select(ur => ur.Role.Name).Distinct().ToList();
            var permissions = userRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToList();

            return (roles, permissions);
        }

        /// <summary>
        /// Deactivate user account
        /// </summary>
        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Activate user account
        /// </summary>
        public async Task<bool> ActivateUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        #region Private Methods

        /// <summary>
        /// Hash password using SHA256
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// Verify password against hash
        /// </summary>
        private static bool VerifyPassword(string password, string hash)
        {
            var computedHash = HashPassword(password);
            return computedHash == hash;
        }

        /// <summary>
        /// Map User entity to UserDto
        /// </summary>
        private static UserDto MapToUserDto(User user, List<string> roles, List<string> permissions)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                AssignedPort = user.Port != null ? new PortDto
                {
                    PortId = user.Port.PortId,
                    Name = user.Port.Name,
                    Location = user.Port.Location
                } : null,
                Roles = roles,
                Permissions = permissions,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }

        #endregion
    }
}