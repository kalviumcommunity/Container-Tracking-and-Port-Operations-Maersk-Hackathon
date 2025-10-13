using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

        public async Task<UsersPagedResponse> GetUsersAsync(UserFilterDto filter)
        {
            try
            {
                // Set defaults for filter parameters
                filter ??= new UserFilterDto();
                var page = Math.Max(1, filter.Page);
                var pageSize = Math.Clamp(filter.PageSize, 1, 100);

                var query = _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .AsQueryable();

                // Filter out deleted users unless specifically requested
                query = query.Where(u => !u.IsDeleted);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(u => 
                        u.Username.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        u.FullName.ToLower().Contains(searchTerm) ||
                        (u.Department != null && u.Department.ToLower().Contains(searchTerm))
                    );
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(filter.Status))
                {
                    switch (filter.Status.ToLower())
                    {
                        case "active":
                            query = query.Where(u => u.IsActive && !u.IsBlocked);
                            break;
                        case "inactive":
                            query = query.Where(u => !u.IsActive);
                            break;
                        case "blocked":
                            query = query.Where(u => u.IsBlocked);
                            break;
                    }
                }

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                    query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name == filter.Role));
                }

                // Apply date filters
                if (filter.RegisteredAfter.HasValue)
                {
                    query = query.Where(u => u.CreatedAt >= filter.RegisteredAfter.Value);
                }

                if (filter.RegisteredBefore.HasValue)
                {
                    query = query.Where(u => u.CreatedAt <= filter.RegisteredBefore.Value);
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
                if (user == null) 
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                    user.FullName = updateDto.FullName.Trim();

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                {
                    var trimmedEmail = updateDto.Email.Trim().ToLower();
                    
                    // Validate email format
                    if (!IsValidEmail(trimmedEmail))
                    {
                        _logger.LogWarning("Invalid email format provided for user {UserId}: {Email}", userId, updateDto.Email);
                        await transaction.RollbackAsync();
                        return false;
                    }
                    
                    // Check if email is already taken by another user (within transaction)
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.UserId != userId && u.Email == trimmedEmail && !u.IsDeleted);
                    if (emailExists) 
                    {
                        _logger.LogWarning("Email already exists for another user: {Email}", trimmedEmail);
                        await transaction.RollbackAsync();
                        return false;
                    }
                    
                    user.Email = trimmedEmail;
                }

                if (updateDto.Department != null)
                    user.Department = string.IsNullOrWhiteSpace(updateDto.Department) ? null : updateDto.Department.Trim();

                if (updateDto.PhoneNumber != null)
                    user.PhoneNumber = string.IsNullOrWhiteSpace(updateDto.PhoneNumber) ? null : updateDto.PhoneNumber.Trim();

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("User {UserId} updated successfully", userId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

        public async Task<bool> DeleteUserAsync(int userId, string reason = "")
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null) return false;

                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                
                // If deleting, make sure user is also blocked
                user.IsBlocked = true;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} deleted with reason: {Reason}", userId, reason);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
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

        /// <summary>
        /// Validate email format and domain
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailAttribute = new EmailAddressAttribute();
                return emailAttribute.IsValid(email) && 
                       Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}