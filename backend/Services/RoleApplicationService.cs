using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    /// <summary>
    /// Service for managing role applications and approvals
    /// </summary>
    public class RoleApplicationService : IRoleApplicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public RoleApplicationService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Submit a new role application
        /// </summary>
        public async Task<RoleApplicationDto> SubmitApplicationAsync(int userId, RoleApplicationRequestDto request)
        {
            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Check if user already has this role
            var hasRole = await _context.UserRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == request.RequestedRole);

            if (hasRole)
            {
                throw new InvalidOperationException("User already has this role");
            }

            // Check if user has a pending application for this role
            var hasPendingApplication = await HasPendingApplicationAsync(userId, request.RequestedRole);
            if (hasPendingApplication)
            {
                throw new InvalidOperationException("User already has a pending application for this role");
            }

            // Validate that the requested role exists
            var roleExists = await _context.Roles.AnyAsync(r => r.Name == request.RequestedRole);
            if (!roleExists)
            {
                throw new ArgumentException("Invalid role name");
            }

            // Create the application
            var application = new RoleApplication
            {
                UserId = userId,
                RequestedRole = request.RequestedRole,
                Justification = request.Justification,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _context.RoleApplications.Add(application);
            await _context.SaveChangesAsync();

            // Return the created application as DTO
            return await GetApplicationAsync(application.ApplicationId) ?? 
                   throw new InvalidOperationException("Failed to retrieve created application");
        }

        /// <summary>
        /// Get all applications submitted by a specific user
        /// </summary>
        public async Task<List<RoleApplicationDto>> GetUserApplicationsAsync(int userId)
        {
            var applications = await _context.RoleApplications
                .Include(ra => ra.User)
                .Include(ra => ra.ReviewedByUser)
                .Where(ra => ra.UserId == userId)
                .OrderByDescending(ra => ra.RequestedAt)
                .ToListAsync();

            return applications.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Get all pending applications (admin only)
        /// </summary>
        public async Task<List<RoleApplicationDto>> GetPendingApplicationsAsync()
        {
            var applications = await _context.RoleApplications
                .Include(ra => ra.User)
                .Include(ra => ra.ReviewedByUser)
                .Where(ra => ra.Status == "Pending")
                .OrderBy(ra => ra.RequestedAt)
                .ToListAsync();

            return applications.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Get all applications with any status (admin only)
        /// </summary>
        public async Task<List<RoleApplicationDto>> GetAllApplicationsAsync()
        {
            var applications = await _context.RoleApplications
                .Include(ra => ra.User)
                .Include(ra => ra.ReviewedByUser)
                .OrderByDescending(ra => ra.RequestedAt)
                .ToListAsync();

            return applications.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Review and approve/reject a role application (admin only)
        /// </summary>
        public async Task<bool> ReviewApplicationAsync(int applicationId, int reviewerId, ReviewApplicationDto review)
        {
            var application = await _context.RoleApplications
                .Include(ra => ra.User)
                .FirstOrDefaultAsync(ra => ra.ApplicationId == applicationId);

            if (application == null)
            {
                throw new ArgumentException("Application not found");
            }

            if (application.Status != "Pending")
            {
                throw new InvalidOperationException("Application has already been reviewed");
            }

            if (review.Status != "Approved" && review.Status != "Rejected")
            {
                throw new ArgumentException("Status must be 'Approved' or 'Rejected'");
            }

            // Update application
            application.Status = review.Status;
            application.ReviewedBy = reviewerId;
            application.ReviewedAt = DateTime.UtcNow;
            application.ReviewNotes = review.ReviewNotes;

            // If approved, grant the role to the user
            if (review.Status == "Approved")
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == application.RequestedRole);
                if (role != null)
                {
                    // Check if user doesn't already have this role
                    var existingUserRole = await _context.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == application.UserId && ur.RoleId == role.RoleId);

                    if (existingUserRole == null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = application.UserId,
                            RoleId = role.RoleId,
                            AssignedAt = DateTime.UtcNow,
                            AssignedByUserId = reviewerId
                        };

                        _context.UserRoles.Add(userRole);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get a specific application by ID
        /// </summary>
        public async Task<RoleApplicationDto?> GetApplicationAsync(int applicationId)
        {
            var application = await _context.RoleApplications
                .Include(ra => ra.User)
                .Include(ra => ra.ReviewedByUser)
                .FirstOrDefaultAsync(ra => ra.ApplicationId == applicationId);

            return application == null ? null : MapToDto(application);
        }

        /// <summary>
        /// Get available roles that a user can apply for
        /// </summary>
        public async Task<List<AvailableRoleDto>> GetAvailableRolesAsync(int userId)
        {
            var allRoles = await _context.Roles.ToListAsync();
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();

            var pendingApplications = await _context.RoleApplications
                .Where(ra => ra.UserId == userId && ra.Status == "Pending")
                .Select(ra => ra.RequestedRole)
                .ToListAsync();

            var availableRoles = new List<AvailableRoleDto>();

            foreach (var role in allRoles)
            {
                var dto = new AvailableRoleDto
                {
                    RoleName = role.Name,
                    Description = GetRoleDescription(role.Name)
                };

                if (userRoles.Contains(role.Name))
                {
                    dto.CanApply = false;
                    dto.ReasonCannotApply = "You already have this role";
                }
                else if (pendingApplications.Contains(role.Name))
                {
                    dto.CanApply = false;
                    dto.ReasonCannotApply = "You have a pending application for this role";
                }
                else if (role.Name == "Admin")
                {
                    dto.CanApply = false;
                    dto.ReasonCannotApply = "Admin role cannot be requested";
                }
                else
                {
                    dto.CanApply = true;
                }

                availableRoles.Add(dto);
            }

            return availableRoles;
        }

        /// <summary>
        /// Check if user has a pending application for a specific role
        /// </summary>
        public async Task<bool> HasPendingApplicationAsync(int userId, string roleName)
        {
            return await _context.RoleApplications
                .AnyAsync(ra => ra.UserId == userId && 
                              ra.RequestedRole == roleName && 
                              ra.Status == "Pending");
        }

        /// <summary>
        /// Cancel a pending application
        /// </summary>
        public async Task<bool> CancelApplicationAsync(int applicationId, int userId)
        {
            var application = await _context.RoleApplications
                .FirstOrDefaultAsync(ra => ra.ApplicationId == applicationId && ra.UserId == userId);

            if (application == null || application.Status != "Pending")
            {
                return false;
            }

            application.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Map RoleApplication entity to DTO
        /// </summary>
        private static RoleApplicationDto MapToDto(RoleApplication application)
        {
            return new RoleApplicationDto
            {
                ApplicationId = application.ApplicationId,
                UserId = application.UserId,
                Username = application.User.Username,
                FullName = application.User.FullName,
                Email = application.User.Email,
                RequestedRole = application.RequestedRole,
                Justification = application.Justification,
                Status = application.Status,
                RequestedAt = application.RequestedAt,
                ReviewedBy = application.ReviewedBy,
                ReviewedByUsername = application.ReviewedByUser?.Username,
                ReviewedByFullName = application.ReviewedByUser?.FullName,
                ReviewedAt = application.ReviewedAt,
                ReviewNotes = application.ReviewNotes
            };
        }

        /// <summary>
        /// Get role description for display
        /// </summary>
        private static string GetRoleDescription(string roleName)
        {
            return roleName switch
            {
                "PortManager" => "Manage port operations, berths, and assign operators to specific areas",
                "Operator" => "Handle container operations, loading/unloading, and berth assignments",
                "Supervisor" => "Supervise port operations and manage operator teams",
                "TrafficController" => "Coordinate ship movements and berth scheduling",
                "Viewer" => "Read-only access to view container and port information",
                "Admin" => "Full system administration access (cannot be requested)",
                _ => "Role-specific permissions and access"
            };
        }
    }
}