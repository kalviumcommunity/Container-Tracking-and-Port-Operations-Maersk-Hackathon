using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Constants;

namespace Backend.Services
{
    /// <summary>
    /// Service for seeding initial data
    /// </summary>
    public interface IDataSeedService
    {
        /// <summary>
        /// Seed initial roles, permissions, and admin user
        /// </summary>
        Task SeedDataAsync();
    }

    /// <summary>
    /// Data seeding service implementation
    /// </summary>
    public class DataSeedService : IDataSeedService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeedService> _logger;

        public DataSeedService(ApplicationDbContext context, ILogger<DataSeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seed initial system data
        /// </summary>
        public async Task SeedDataAsync()
        {
            _logger.LogInformation("Starting data seeding process");
            
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Seed permissions first
                await SeedPermissionsAsync();

                // Seed roles
                await SeedRolesAsync();

                // Seed role-permission relationships
                await SeedRolePermissionsAsync();

                // Seed default admin user
                await SeedAdminUserAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding");
                throw;
            }
        }

        /// <summary>
        /// Seed system permissions
        /// </summary>
        private async Task SeedPermissionsAsync()
        {
            var existingPermissions = await _context.Permissions.Select(p => p.Name).ToListAsync();

            var permissionsToSeed = new List<Permission>
            {
                // Global permissions
                new Permission { Name = Permissions.GlobalPortAccess, Description = "Access to all ports in the system", Category = "Global", IsSystemPermission = true },
                new Permission { Name = Permissions.ManageAllPorts, Description = "Manage all ports in the system", Category = "Global", IsSystemPermission = true },
                new Permission { Name = Permissions.ManageUsers, Description = "Create, update, and delete users", Category = "User Management", IsSystemPermission = true },
                new Permission { Name = Permissions.ManageRoles, Description = "Create, update, and delete roles", Category = "User Management", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewSystemReports, Description = "View system-wide reports and analytics", Category = "Reports", IsSystemPermission = true },

                // Port management permissions
                new Permission { Name = Permissions.ManagePortDetails, Description = "Update port information and settings", Category = "Port Management", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewPortDetails, Description = "View port information and details", Category = "Port Management", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewPortReports, Description = "View port-specific reports", Category = "Reports", IsSystemPermission = true },

                // Container permissions
                new Permission { Name = Permissions.ManageContainers, Description = "Create, update, and delete container records", Category = "Container Operations", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewContainers, Description = "View container information", Category = "Container Operations", IsSystemPermission = true },
                new Permission { Name = Permissions.TrackContainers, Description = "Track container movements and status", Category = "Container Operations", IsSystemPermission = true },

                // Ship permissions
                new Permission { Name = Permissions.ManageShips, Description = "Create, update, and delete ship records", Category = "Ship Operations", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewShips, Description = "View ship information", Category = "Ship Operations", IsSystemPermission = true },
                new Permission { Name = Permissions.ScheduleShips, Description = "Schedule ship arrivals and departures", Category = "Ship Operations", IsSystemPermission = true },

                // Cargo permissions
                new Permission { Name = Permissions.ManageCargo, Description = "Create, update, and delete cargo records", Category = "Cargo Operations", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewCargo, Description = "View cargo information", Category = "Cargo Operations", IsSystemPermission = true },

                // Berth permissions
                new Permission { Name = Permissions.ManageBerths, Description = "Create, update, and delete berth records", Category = "Berth Management", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewBerths, Description = "View berth information", Category = "Berth Management", IsSystemPermission = true },
                new Permission { Name = Permissions.AllocateBerths, Description = "Allocate berths to ships", Category = "Berth Management", IsSystemPermission = true },

                // Equipment permissions
                new Permission { Name = Permissions.ManageEquipment, Description = "Create, update, and delete equipment records", Category = "Equipment Management", IsSystemPermission = true },
                new Permission { Name = Permissions.ViewEquipment, Description = "View equipment information", Category = "Equipment Management", IsSystemPermission = true }
            };

            foreach (var permission in permissionsToSeed)
            {
                if (!existingPermissions.Contains(permission.Name))
                {
                    _context.Permissions.Add(permission);
                    _logger.LogInformation($"Added permission: {permission.Name}");
                }
            }
        }

        /// <summary>
        /// Seed system roles
        /// </summary>
        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Starting role seeding");
            
            try
            {
                var existingRoles = await _context.Roles.Select(r => r.Name).ToListAsync();

                var rolesToSeed = new List<Role>
                {
                    new Role 
                    { 
                        Name = Roles.Admin, 
                        Description = "System administrator with full access to all features and data", 
                        IsSystemRole = true 
                    },
                    new Role 
                    { 
                        Name = Roles.PortManager, 
                        Description = "Port manager with administrative access to assigned port operations", 
                        IsSystemRole = true 
                    },
                    new Role 
                    { 
                        Name = Roles.Operator, 
                        Description = "Port operator with access to operational features for assigned port", 
                        IsSystemRole = true 
                    },
                    new Role 
                    { 
                        Name = Roles.Viewer, 
                        Description = "Read-only access to port data and reports", 
                        IsSystemRole = true 
                    }
                };
                
                foreach (var role in rolesToSeed)
                {
                    if (!existingRoles.Contains(role.Name))
                    {
                        _context.Roles.Add(role);
                        _logger.LogInformation($"Added role: {role.Name}");
                    }
                }
                
                _logger.LogInformation("Role seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during role seeding");
                throw;
            }
        }

        /// <summary>
        /// Seed role-permission relationships
        /// </summary>
        private async Task SeedRolePermissionsAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            var permissions = await _context.Permissions.ToListAsync();
            var existingRolePermissions = await _context.RolePermissions.ToListAsync();

            // Define role-permission mappings
            var rolePermissionMappings = new Dictionary<string, string[]>
            {
                [Roles.Admin] = Permissions.All,
                [Roles.PortManager] = new[]
                {
                    Permissions.ViewPortDetails, Permissions.ManagePortDetails, Permissions.ViewPortReports,
                    Permissions.ViewContainers, Permissions.ManageContainers, Permissions.TrackContainers,
                    Permissions.ViewShips, Permissions.ManageShips, Permissions.ScheduleShips,
                    Permissions.ViewCargo, Permissions.ManageCargo,
                    Permissions.ViewBerths, Permissions.ManageBerths, Permissions.AllocateBerths,
                    Permissions.ViewEquipment, Permissions.ManageEquipment
                },
                [Roles.Operator] = new[]
                {
                    Permissions.ViewPortDetails, Permissions.ViewPortReports,
                    Permissions.ViewContainers, Permissions.TrackContainers,
                    Permissions.ViewShips, Permissions.ScheduleShips,
                    Permissions.ViewCargo, Permissions.ManageCargo,
                    Permissions.ViewBerths, Permissions.AllocateBerths,
                    Permissions.ViewBerthAssignments,
                    Permissions.ViewEquipment
                },
                [Roles.Viewer] = new[]
                {
                    Permissions.ViewPortDetails, Permissions.ViewPortReports,
                    Permissions.ViewContainers, Permissions.TrackContainers,
                    Permissions.ViewShips, Permissions.ViewCargo,
                    Permissions.ViewBerths, Permissions.ViewEquipment
                }
            };

            foreach (var mapping in rolePermissionMappings)
            {
                var role = roles.FirstOrDefault(r => r.Name == mapping.Key);
                if (role == null) continue;

                foreach (var permissionName in mapping.Value)
                {
                    var permission = permissions.FirstOrDefault(p => p.Name == permissionName);
                    if (permission == null) continue;

                    // Check if this role-permission relationship already exists
                    if (!existingRolePermissions.Any(rp => rp.RoleId == role.RoleId && rp.PermissionId == permission.PermissionId))
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = role.RoleId,
                            PermissionId = permission.PermissionId,
                            GrantedAt = DateTime.UtcNow
                        });
                        _logger.LogInformation($"Added permission '{permissionName}' to role '{role.Name}'");
                    }
                }
            }
        }

        /// <summary>
        /// Seed default admin user
        /// </summary>
        private async Task SeedAdminUserAsync()
        {
            const string adminUsername = "admin";
            const string adminEmail = "admin@example.com";
            const string defaultPassword = "Admin123!";

            var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Username == adminUsername);
            if (existingAdmin != null)
            {
                _logger.LogInformation("Admin user already exists");
                // Update the password to ensure it matches our current hashing
                var newHash = HashPassword(defaultPassword);
                if (existingAdmin.PasswordHash != newHash)
                {
                    existingAdmin.PasswordHash = newHash;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Admin password synchronized");
                }
                return;
            }

            // Create admin user
            var adminUser = new User
            {
                Username = adminUsername,
                Email = adminEmail,
                PasswordHash = HashPassword(defaultPassword),
                FullName = "System Administrator",
                Department = "IT Administration",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            // Assign admin role
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Admin);
            if (adminRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Created admin user: {adminUsername} with default password");
            }
        }

        /// <summary>
        /// Hash password using SHA256
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}