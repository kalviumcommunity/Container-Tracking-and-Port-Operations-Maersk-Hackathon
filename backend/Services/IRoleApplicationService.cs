using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    /// <summary>
    /// Interface for role application service operations
    /// </summary>
    public interface IRoleApplicationService
    {
        /// <summary>
        /// Submit a new role application
        /// </summary>
        Task<RoleApplicationDto> SubmitApplicationAsync(int userId, RoleApplicationRequestDto request);
        
        /// <summary>
        /// Get all applications submitted by a specific user
        /// </summary>
        Task<List<RoleApplicationDto>> GetUserApplicationsAsync(int userId);
        
        /// <summary>
        /// Get all pending applications (admin only)
        /// </summary>
        Task<List<RoleApplicationDto>> GetPendingApplicationsAsync();
        
        /// <summary>
        /// Get all applications with any status (admin only)
        /// </summary>
        Task<List<RoleApplicationDto>> GetAllApplicationsAsync();
        
        /// <summary>
        /// Review and approve/reject a role application (admin only)
        /// </summary>
        Task<bool> ReviewApplicationAsync(int applicationId, int reviewerId, ReviewApplicationDto review);
        
        /// <summary>
        /// Get a specific application by ID
        /// </summary>
        Task<RoleApplicationDto?> GetApplicationAsync(int applicationId);
        
        /// <summary>
        /// Get available roles that a user can apply for
        /// </summary>
        Task<List<AvailableRoleDto>> GetAvailableRolesAsync(int userId);
        
        /// <summary>
        /// Check if user has a pending application for a specific role
        /// </summary>
        Task<bool> HasPendingApplicationAsync(int userId, string roleName);
        
        /// <summary>
        /// Cancel a pending application
        /// </summary>
        Task<bool> CancelApplicationAsync(int applicationId, int userId);
    }
}