using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    /// <summary>
    /// Service for managing users in the admin dashboard
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(ApplicationDbContext context, ILogger<UserManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UsersPagedResponse> GetUsersAsync(int page = 1, int pageSize = 20, string? searchTerm = null, bool includeDeleted = false)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .AsQueryable();

                // Filter out deleted users unless specifically requested
                if (!includeDeleted)
                {
                    query = query.Where(u => !u.IsDeleted);
                }

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(u => 
                        u.Username.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        u.FullName.ToLower().Contains(searchTerm) ||
                        (u.Department != null && u.Department.ToLower().Contains(searchTerm))
                    );
                }

                var total = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)total / pageSize);

                var users = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserListDto
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        Department = u.Department,
                        PhoneNumber = u.PhoneNumber,
                        IsActive = u.IsActive,
                        IsBlocked = u.IsBlocked,
                        IsDeleted = u.IsDeleted,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        LastLoginAt = u.LastLoginAt,
                        Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                    })
                    .ToListAsync();

                return new UsersPagedResponse
                {
                    Users = users,
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users list");
                throw;
            }
        }

        public async Task<UserListDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

                if (user == null) return null;

                return new UserListDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Department = user.Department,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    IsBlocked = user.IsBlocked,
                    IsDeleted = user.IsDeleted,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    LastLoginAt = user.LastLoginAt,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
                if (user == null) return false;

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                    user.FullName = updateDto.FullName.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                {
                    // Check if email is already taken by another user
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.UserId != userId && u.Email == updateDto.Email && !u.IsDeleted);
                    if (emailExists) return false;
                    
                    user.Email = updateDto.Email.Trim().ToLower();
                }

                if (updateDto.Department != null)
                    user.Department = string.IsNullOrWhiteSpace(updateDto.Department) ? null : updateDto.Department.Trim();

                if (updateDto.PhoneNumber != null)
                    user.PhoneNumber = string.IsNullOrWhiteSpace(updateDto.PhoneNumber) ? null : updateDto.PhoneNumber.Trim();

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} updated successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserRolesAsync(int userId, UpdateUserRolesDto rolesDto)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

                if (user == null) return false;

                // Get all valid role IDs
                var validRoles = await _context.Roles
                    .Where(r => rolesDto.Roles.Contains(r.Name))
                    .ToListAsync();

                if (validRoles.Count != rolesDto.Roles.Count)
                {
                    _logger.LogWarning("Some roles not found for user {UserId}", userId);
                    return false;
                }

                // Remove existing roles
                _context.UserRoles.RemoveRange(user.UserRoles);

                // Add new roles
                foreach (var role in validRoles)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = role.RoleId
                    });
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} roles updated successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating roles for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> BlockUserAsync(int userId, bool isBlocked)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
                if (user == null) return false;

                user.IsBlocked = isBlocked;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} {Action}", userId, isBlocked ? "blocked" : "unblocked");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating block status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId, bool isDeleted)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null) return false;

                user.IsDeleted = isDeleted;
                user.UpdatedAt = DateTime.UtcNow;
                
                // If restoring, make sure user is not blocked
                if (!isDeleted)
                {
                    user.IsBlocked = false;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} {Action}", userId, isDeleted ? "deleted" : "restored");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delete status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, UpdateUserStatusDto statusDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
                if (user == null) return false;

                user.IsActive = statusDto.IsActive;
                user.IsBlocked = statusDto.IsBlocked;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} status updated successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<SystemStatsDto> GetSystemStatsAsync()
        {
            try
            {
                var stats = new SystemStatsDto
                {
                    Users = await _context.Users.CountAsync(u => !u.IsDeleted),
                    Containers = await _context.Containers.CountAsync(),
                    Ships = await _context.Ships.CountAsync(),
                    Berths = await _context.Berths.CountAsync(),
                    Ports = await _context.Ports.CountAsync(),
                    ActiveUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.IsActive && !u.IsBlocked),
                    PendingRoleApplications = await _context.RoleApplications.CountAsync(ra => ra.Status == "Pending"),
                    BlockedUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.IsBlocked),
                    DeletedUsers = await _context.Users.CountAsync(u => u.IsDeleted)
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system statistics");
                throw;
            }
        }

        public async Task<List<string>> GetAvailableRolesAsync()
        {
            try
            {
                return await _context.Roles
                    .Select(r => r.Name)
                    .OrderBy(name => name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available roles");
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.UserId == userId && !u.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} exists", userId);
                return false;
            }
        }
    }
}