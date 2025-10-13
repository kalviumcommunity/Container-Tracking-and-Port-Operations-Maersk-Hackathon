using Backend.DTOs;

namespace Backend.Services
{
    /// <summary>
    /// Interface for user management operations
    /// </summary>
    public interface IUserManagementService
    {
        /// <summary>
        /// Get paginated list of users with filtering options
        /// </summary>
        Task<UsersPagedResponse> GetUsersAsync(UserFilterDto filter);

        /// <summary>
        /// Get detailed user information by ID
        /// </summary>
        Task<UserListDto?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Update user information
        /// </summary>
        Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateDto);

        /// <summary>
        /// Update user roles
        /// </summary>
        Task<bool> UpdateUserRolesAsync(int userId, UpdateUserRolesDto rolesDto);

        /// <summary>
        /// Block or unblock a user
        /// </summary>
        Task<bool> BlockUserAsync(int userId, bool isBlocked);

        /// <summary>
        /// Soft delete or restore a user
        /// </summary>
        Task<bool> DeleteUserAsync(int userId, string reason = "");

        /// <summary>
        /// Update user status (active/inactive)
        /// </summary>
        Task<bool> UpdateUserStatusAsync(int userId, UpdateUserStatusDto statusDto);

        /// <summary>
        /// Get system statistics for admin dashboard
        /// </summary>
        Task<SystemStatsDto> GetSystemStatsAsync();

        /// <summary>
        /// Get all available roles in the system
        /// </summary>
        Task<List<string>> GetAvailableRolesAsync();

        /// <summary>
        /// Check if a user exists and is not deleted
        /// </summary>
        Task<bool> UserExistsAsync(int userId);
    }
}