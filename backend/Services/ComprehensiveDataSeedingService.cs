using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services
{
    /// <summary>
    /// Comprehensive data seeding service with realistic maritime industry data
    /// </summary>
    public class ComprehensiveDataSeedingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprehensiveDataSeedingService> _logger;
        private readonly Random _random;

        public ComprehensiveDataSeedingService(
            ApplicationDbContext context, 
            ILogger<ComprehensiveDataSeedingService> logger)
        {
            _context = context;
            _logger = logger;
            _random = new Random();
        }

        /// <summary>
        /// Seeds all data if database is empty
        /// </summary>
        public async Task SeedAllAsync(bool forceReseed = false)
        {
            _logger.LogInformation("Starting comprehensive data seeding...");

            try
            {
                // Check if we need to seed: only skip when ALL key business tables already have data
                if (!forceReseed)
                {
                    var hasUsers = await _context.Users.AnyAsync();
                    var hasPorts = await _context.Ports.AnyAsync();
                    var hasShips = await _context.Ships.AnyAsync();
                    var hasBerths = await _context.Berths.AnyAsync();
                    var hasContainers = await _context.Containers.AnyAsync();

                    if (hasUsers && hasPorts && hasShips && hasBerths && hasContainers)
                    {
                        _logger.LogInformation("Database already contains users and business data. Skipping seeding.");
                        return;
                    }
                }

                if (forceReseed)
                {
                    _logger.LogWarning("Force reseeding requested. Clearing existing data...");
                    await ClearAllDataAsync();
                }

                // Seed in proper order (respecting foreign key dependencies)
                await SeedRolesAndPermissionsAsync();
                await SeedPortsAsync();
                await SeedUsersAsync();
                await SeedShippingLinesAsync();
                await SeedShipsAsync();
                await SeedBerthsAsync();
                await SeedContainerTypesAsync();
                await SeedContainersAsync();
                await SeedBerthAssignmentsAsync();
                await SeedShipContainerOperationsAsync();
                await SeedContainerMovementsAsync();
                await SeedEventsAsync();
                await SeedAnalyticsDataAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("Comprehensive data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during comprehensive data seeding");
                throw;
            }
        }

        private async Task ClearAllDataAsync()
        {
            // Clear in reverse dependency order
            _context.Analytics.RemoveRange(_context.Analytics);
            _context.Events.RemoveRange(_context.Events);
            _context.ContainerMovements.RemoveRange(_context.ContainerMovements);
            _context.ShipContainers.RemoveRange(_context.ShipContainers);
            _context.BerthAssignments.RemoveRange(_context.BerthAssignments);
            _context.Containers.RemoveRange(_context.Containers);
            _context.Berths.RemoveRange(_context.Berths);
            _context.Ships.RemoveRange(_context.Ships);
            _context.Ports.RemoveRange(_context.Ports);
            _context.UserRoles.RemoveRange(_context.UserRoles);
            _context.RolePermissions.RemoveRange(_context.RolePermissions);
            _context.Users.RemoveRange(_context.Users);
            _context.Roles.RemoveRange(_context.Roles);
            _context.Permissions.RemoveRange(_context.Permissions);

            await _context.SaveChangesAsync();
        }

        private async Task SeedRolesAndPermissionsAsync()
        {
            _logger.LogInformation("Seeding roles and permissions...");

            // Define the full set of permissions we expect in the system
            var permissions = new List<Permission>
            {
                // Global Permissions
                new Permission { Name = "GlobalPortAccess", Description = "Access to all ports globally", Category = "Global" },
                new Permission { Name = "ManageAllPorts", Description = "Manage all ports in the system", Category = "Global" },
                new Permission { Name = "ManageUsers", Description = "Create, update, and delete users", Category = "Global" },
                new Permission { Name = "ManageRoles", Description = "Assign and modify user roles", Category = "Global" },
                new Permission { Name = "SeedData", Description = "Seed database with test data", Category = "Global" },

                // Container Permissions
                new Permission { Name = "ViewContainers", Description = "View container information", Category = "Containers" },
                new Permission { Name = "ManageContainers", Description = "Create, update, delete containers", Category = "Containers" },
                new Permission { Name = "TrackContainers", Description = "Track container movements and status", Category = "Containers" },

                // Ship Permissions
                new Permission { Name = "ViewShips", Description = "View ship information", Category = "Ships" },
                new Permission { Name = "ManageShips", Description = "Create, update, delete ships", Category = "Ships" },
                new Permission { Name = "ScheduleShips", Description = "Schedule ship arrivals and departures", Category = "Ships" },

                // Port Permissions
                new Permission { Name = "ViewPortDetails", Description = "View port information and statistics", Category = "Ports" },
                new Permission { Name = "ManagePortDetails", Description = "Update port configuration", Category = "Ports" },
                new Permission { Name = "ViewPortReports", Description = "Access port performance reports", Category = "Ports" },

                // Berth Permissions
                new Permission { Name = "ViewBerths", Description = "View berth information", Category = "Berths" },
                new Permission { Name = "ManageBerths", Description = "Create, update berth configurations", Category = "Berths" },
                new Permission { Name = "AllocateBerths", Description = "Assign berths to ships", Category = "Berths" },
                new Permission { Name = "ViewBerthAssignments", Description = "View berth assignment schedules", Category = "Berths" },
                new Permission { Name = "ManageBerthAssignments", Description = "Create and modify berth assignments", Category = "Berths" },

                // Cargo Permissions
                new Permission { Name = "ViewCargo", Description = "View cargo and container contents", Category = "Cargo" },
                new Permission { Name = "ManageCargo", Description = "Manage cargo operations", Category = "Cargo" },

                // Equipment Permissions
                new Permission { Name = "ViewEquipment", Description = "View port equipment status", Category = "Equipment" },
                new Permission { Name = "ManageEquipment", Description = "Manage port equipment", Category = "Equipment" },

                // Analytics Permissions
                new Permission { Name = "ViewDashboard", Description = "Access dashboard and metrics", Category = "Analytics" },
                new Permission { Name = "ViewReports", Description = "Generate and view reports", Category = "Analytics" },
                new Permission { Name = "GenerateReports", Description = "Generate custom reports", Category = "Analytics" },
                new Permission { Name = "ExportData", Description = "Export data and reports", Category = "Analytics" }
            };

            // Insert only missing permissions (idempotent)
            var existingPermissionNames = await _context.Permissions
                .AsNoTracking()
                .Select(p => p.Name)
                .ToListAsync();

            var newPermissions = permissions
                .Where(p => !existingPermissionNames.Contains(p.Name))
                .ToList();

            if (newPermissions.Count > 0)
            {
                _context.Permissions.AddRange(newPermissions);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} new permissions", newPermissions.Count);
            }
            else
            {
                _logger.LogInformation("No new permissions to insert (already up-to-date)");
            }

            // Define the full set of roles
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Admin",
                    Description = "System administrator with full access",
                    IsSystemRole = true
                },
                new Role
                {
                    Name = "PortManager",
                    Description = "Port manager with port-specific management access",
                    IsSystemRole = true
                },
                new Role
                {
                    Name = "Operator",
                    Description = "Operational staff for daily container and ship operations",
                    IsSystemRole = true
                },
                new Role
                {
                    Name = "Viewer",
                    Description = "Read-only access to view system information",
                    IsSystemRole = true
                }
            };

            // Insert only missing roles (idempotent)
            var existingRoleNames = await _context.Roles
                .AsNoTracking()
                .Select(r => r.Name)
                .ToListAsync();

            var newRoles = roles
                .Where(r => !existingRoleNames.Contains(r.Name))
                .ToList();

            if (newRoles.Count > 0)
            {
                _context.Roles.AddRange(newRoles);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} new roles", newRoles.Count);
            }
            else
            {
                _logger.LogInformation("No new roles to insert (already up-to-date)");
            }

            // Reload canonical lists with IDs from DB
            var allPermissionsByName = await _context.Permissions
                .AsNoTracking()
                .ToDictionaryAsync(p => p.Name, p => new { p.PermissionId });

            var adminRole = await _context.Roles.AsNoTracking().FirstAsync(r => r.Name == "Admin");
            var portManagerRole = await _context.Roles.AsNoTracking().FirstAsync(r => r.Name == "PortManager");
            var operatorRole = await _context.Roles.AsNoTracking().FirstAsync(r => r.Name == "Operator");
            var viewerRole = await _context.Roles.AsNoTracking().FirstAsync(r => r.Name == "Viewer");

            var rolePermissionsToAdd = new List<RolePermission>();

            // Admin gets all permissions
            foreach (var permName in allPermissionsByName.Keys)
            {
                rolePermissionsToAdd.Add(new RolePermission { RoleId = adminRole.RoleId, PermissionId = allPermissionsByName[permName].PermissionId });
            }

            // PortManager permissions
            var portManagerPermissions = new[]
            {
                "ViewContainers", "ManageContainers", "TrackContainers",
                "ViewShips", "ManageShips", "ScheduleShips",
                "ViewPortDetails", "ManagePortDetails", "ViewPortReports",
                "ViewBerths", "ManageBerths", "AllocateBerths", "ViewBerthAssignments", "ManageBerthAssignments",
                "ViewCargo", "ManageCargo",
                "ViewEquipment", "ManageEquipment",
                "ViewDashboard", "ViewReports", "GenerateReports", "ExportData"
            };

            foreach (var permName in portManagerPermissions)
            {
                if (allPermissionsByName.TryGetValue(permName, out var p))
                {
                    rolePermissionsToAdd.Add(new RolePermission { RoleId = portManagerRole.RoleId, PermissionId = p.PermissionId });
                }
            }

            // Operator permissions
            var operatorPermissions = new[]
            {
                "ViewContainers", "ManageContainers", "TrackContainers",
                "ViewShips", "ViewPortDetails",
                "ViewBerths", "ViewBerthAssignments",
                "ViewCargo", "ManageCargo",
                "ViewEquipment",
                "ViewDashboard", "ViewReports"
            };

            foreach (var permName in operatorPermissions)
            {
                if (allPermissionsByName.TryGetValue(permName, out var p))
                {
                    rolePermissionsToAdd.Add(new RolePermission { RoleId = operatorRole.RoleId, PermissionId = p.PermissionId });
                }
            }

            // Viewer permissions
            var viewerPermissions = new[]
            {
                "ViewContainers", "ViewShips", "ViewPortDetails",
                "ViewBerths", "ViewBerthAssignments",
                "ViewCargo", "ViewEquipment",
                "ViewDashboard", "ViewReports"
            };

            foreach (var permName in viewerPermissions)
            {
                if (allPermissionsByName.TryGetValue(permName, out var p))
                {
                    rolePermissionsToAdd.Add(new RolePermission { RoleId = viewerRole.RoleId, PermissionId = p.PermissionId });
                }
            }

            // Insert only missing role-permission links (idempotent)
            var existingRolePermPairs = await _context.RolePermissions
                .AsNoTracking()
                .Select(rp => new { rp.RoleId, rp.PermissionId })
                .ToListAsync();

            var existingSet = new HashSet<(int RoleId, int PermissionId)>(existingRolePermPairs.Select(x => (x.RoleId, x.PermissionId)));
            var newRolePerms = rolePermissionsToAdd
                .Where(rp => !existingSet.Contains((rp.RoleId, rp.PermissionId)))
                .ToList();

            if (newRolePerms.Count > 0)
            {
                _context.RolePermissions.AddRange(newRolePerms);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} new role-permission mappings", newRolePerms.Count);
            }
            else
            {
                _logger.LogInformation("No new role-permission mappings to insert (already up-to-date)");
            }
        }

        private async Task SeedPortsAsync()
        {
            _logger.LogInformation("Seeding world ports...");

            var ports = new List<Port>
            {
                // Asia-Pacific
                new Port { Name = "Port of Shanghai", Code = "CNSHA", Country = "China", Location = "Shanghai, China", TotalContainerCapacity = 47000000, CurrentContainerCount = 35000000, Status = "Operational", TimeZone = "Asia/Shanghai" },
                new Port { Name = "Port of Singapore", Code = "SGSIN", Country = "Singapore", Location = "Singapore", TotalContainerCapacity = 37200000, CurrentContainerCount = 28000000, Status = "Operational", TimeZone = "Asia/Singapore" },
                new Port { Name = "Port of Ningbo-Zhoushan", Code = "CNNGB", Country = "China", Location = "Ningbo, China", TotalContainerCapacity = 31000000, CurrentContainerCount = 23000000, Status = "Operational", TimeZone = "Asia/Shanghai" },
                new Port { Name = "Port of Shenzhen", Code = "CNSZN", Country = "China", Location = "Shenzhen, China", TotalContainerCapacity = 28000000, CurrentContainerCount = 21000000, Status = "Operational", TimeZone = "Asia/Shanghai" },
                new Port { Name = "Port of Guangzhou", Code = "CNGZH", Country = "China", Location = "Guangzhou, China", TotalContainerCapacity = 24000000, CurrentContainerCount = 18000000, Status = "Operational", TimeZone = "Asia/Shanghai" },
                new Port { Name = "Port of Busan", Code = "KRPUS", Country = "South Korea", Location = "Busan, South Korea", TotalContainerCapacity = 22000000, CurrentContainerCount = 16500000, Status = "Operational", TimeZone = "Asia/Seoul" },
                new Port { Name = "Port of Hong Kong", Code = "HKHKG", Country = "Hong Kong", Location = "Hong Kong", TotalContainerCapacity = 18000000, CurrentContainerCount = 13500000, Status = "Operational", TimeZone = "Asia/Hong_Kong" },
                new Port { Name = "Port of Qingdao", Code = "CNTAO", Country = "China", Location = "Qingdao, China", TotalContainerCapacity = 21000000, CurrentContainerCount = 15800000, Status = "Operational", TimeZone = "Asia/Shanghai" },
                new Port { Name = "Port of Tokyo", Code = "JPTYO", Country = "Japan", Location = "Tokyo, Japan", TotalContainerCapacity = 5200000, CurrentContainerCount = 3900000, Status = "Operational", TimeZone = "Asia/Tokyo" },

                // Europe
                new Port { Name = "Port of Rotterdam", Code = "NLRTM", Country = "Netherlands", Location = "Rotterdam, Netherlands", TotalContainerCapacity = 14800000, CurrentContainerCount = 11100000, Status = "Operational", TimeZone = "Europe/Amsterdam" },
                new Port { Name = "Port of Antwerp", Code = "BEANR", Country = "Belgium", Location = "Antwerp, Belgium", TotalContainerCapacity = 12000000, CurrentContainerCount = 9000000, Status = "Operational", TimeZone = "Europe/Brussels" },
                new Port { Name = "Port of Hamburg", Code = "DEHAM", Country = "Germany", Location = "Hamburg, Germany", TotalContainerCapacity = 8700000, CurrentContainerCount = 6500000, Status = "Operational", TimeZone = "Europe/Berlin" },
                new Port { Name = "Port of Valencia", Code = "ESVLC", Country = "Spain", Location = "Valencia, Spain", TotalContainerCapacity = 5400000, CurrentContainerCount = 4000000, Status = "Operational", TimeZone = "Europe/Madrid" },
                new Port { Name = "Port of Piraeus", Code = "GRPIR", Country = "Greece", Location = "Piraeus, Greece", TotalContainerCapacity = 5600000, CurrentContainerCount = 4200000, Status = "Operational", TimeZone = "Europe/Athens" },
                new Port { Name = "Port of Felixstowe", Code = "GBFXT", Country = "United Kingdom", Location = "Felixstowe, UK", TotalContainerCapacity = 4000000, CurrentContainerCount = 3000000, Status = "Operational", TimeZone = "Europe/London" },

                // North America
                new Port { Name = "Port of Los Angeles", Code = "USLAX", Country = "United States", Location = "Los Angeles, CA, USA", TotalContainerCapacity = 9200000, CurrentContainerCount = 6900000, Status = "Operational", TimeZone = "America/Los_Angeles" },
                new Port { Name = "Port of Long Beach", Code = "USLGB", Country = "United States", Location = "Long Beach, CA, USA", TotalContainerCapacity = 8100000, CurrentContainerCount = 6000000, Status = "Operational", TimeZone = "America/Los_Angeles" },
                new Port { Name = "Port of New York and New Jersey", Code = "USNYC", Country = "United States", Location = "New York, NY, USA", TotalContainerCapacity = 7200000, CurrentContainerCount = 5400000, Status = "Operational", TimeZone = "America/New_York" },
                new Port { Name = "Port of Savannah", Code = "USSAV", Country = "United States", Location = "Savannah, GA, USA", TotalContainerCapacity = 4700000, CurrentContainerCount = 3500000, Status = "Operational", TimeZone = "America/New_York" },
                new Port { Name = "Port of Vancouver", Code = "CAVAN", Country = "Canada", Location = "Vancouver, BC, Canada", TotalContainerCapacity = 3500000, CurrentContainerCount = 2600000, Status = "Operational", TimeZone = "America/Vancouver" },

                // Middle East & Africa
                new Port { Name = "Port of Dubai", Code = "AEDXB", Country = "United Arab Emirates", Location = "Dubai, UAE", TotalContainerCapacity = 15000000, CurrentContainerCount = 11200000, Status = "Operational", TimeZone = "Asia/Dubai" },
                new Port { Name = "Port Said East", Code = "EGPSE", Country = "Egypt", Location = "Port Said, Egypt", TotalContainerCapacity = 3500000, CurrentContainerCount = 2600000, Status = "Operational", TimeZone = "Africa/Cairo" },
                new Port { Name = "Port of Jeddah", Code = "SAJED", Country = "Saudi Arabia", Location = "Jeddah, Saudi Arabia", TotalContainerCapacity = 2000000, CurrentContainerCount = 1500000, Status = "Operational", TimeZone = "Asia/Riyadh" },

                // South America
                new Port { Name = "Port of Santos", Code = "BRSSZ", Country = "Brazil", Location = "Santos, Brazil", TotalContainerCapacity = 4200000, CurrentContainerCount = 3100000, Status = "Operational", TimeZone = "America/Sao_Paulo" },
                new Port { Name = "Port of Callao", Code = "PECLL", Country = "Peru", Location = "Callao, Peru", TotalContainerCapacity = 2300000, CurrentContainerCount = 1700000, Status = "Operational", TimeZone = "America/Lima" }
            };

            // Ensure required fields are populated and insert only missing ports by Code (idempotent)
            foreach (var port in ports)
            {
                // Required string fields defaults
                port.Coordinates = string.IsNullOrWhiteSpace(port.Coordinates) ? GenerateRealisticCoordinates() : port.Coordinates;
                port.OperatingHours = string.IsNullOrWhiteSpace(port.OperatingHours) ? "24/7" : port.OperatingHours;
                port.Services = string.IsNullOrWhiteSpace(port.Services) ? "Container" : port.Services;
                port.ContactInfo = string.IsNullOrWhiteSpace(port.ContactInfo) ? $"Operations Desk, {port.Name}" : port.ContactInfo;

                // Reasonable numeric defaults
                if (port.MaxShipCapacity <= 0)
                {
                    port.MaxShipCapacity = port.TotalContainerCapacity switch
                    {
                        > 30000000 => 120,
                        > 15000000 => 80,
                        > 5000000 => 40,
                        _ => 20
                    };
                }

                if (port.CurrentShipCount < 0 || port.CurrentShipCount > port.MaxShipCapacity)
                {
                    port.CurrentShipCount = _random.Next(0, Math.Max(1, port.MaxShipCapacity / 3));
                }

                port.CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 365));
                port.UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 24));
            }

            var existingCodes = await _context.Ports.AsNoTracking().Select(p => p.Code).ToListAsync();
            var newPorts = ports.Where(p => !existingCodes.Contains(p.Code)).ToList();

            if (newPorts.Count > 0)
            {
                _context.Ports.AddRange(newPorts);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} new ports", newPorts.Count);
            }
            else
            {
                _logger.LogInformation("No new ports to insert (already up-to-date)");
            }
        }

        private async Task SeedUsersAsync()
        {
            _logger.LogInformation("Seeding users...");

            var adminRole = await _context.Roles.FirstAsync(r => r.Name == "Admin");
            var portManagerRole = await _context.Roles.FirstAsync(r => r.Name == "PortManager");
            var operatorRole = await _context.Roles.FirstAsync(r => r.Name == "Operator");
            var viewerRole = await _context.Roles.FirstAsync(r => r.Name == "Viewer");

            var ports = await _context.Ports.ToListAsync();

            var desiredUsers = new List<User>
            {
                // SECURITY: Use environment variables for default passwords in production
                new User { Username = "admin", Email = "admin@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("ADMIN_DEFAULT_PASSWORD") ?? "TempAdmin123!ChangeMePlease"), FullName = "System Administrator", PhoneNumber = "+1-555-0001", Department = "IT Administration", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-365), LastLoginAt = DateTime.UtcNow.AddMinutes(-30) },
                new User { Username = "john.harbor", Email = "john.harbor@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "John Harbor", PhoneNumber = "+1-555-0101", Department = "Port Operations", PortId = ports.First(p => p.Code == "USLAX").PortId, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-200), LastLoginAt = DateTime.UtcNow.AddHours(-2) },
                new User { Username = "maria.santos", Email = "maria.santos@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "Maria Santos", PhoneNumber = "+31-20-555-0201", Department = "Port Operations", PortId = ports.First(p => p.Code == "NLRTM").PortId, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-180), LastLoginAt = DateTime.UtcNow.AddHours(-4) },
                new User { Username = "chen.wei", Email = "chen.wei@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "Chen Wei", PhoneNumber = "+86-21-555-0301", Department = "Port Operations", PortId = ports.First(p => p.Code == "CNSHA").PortId, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-150), LastLoginAt = DateTime.UtcNow.AddHours(-1) },
                new User { Username = "sarah.crane", Email = "sarah.crane@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "Sarah Crane", PhoneNumber = "+1-555-0401", Department = "Container Operations", PortId = ports.First(p => p.Code == "USLAX").PortId, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-120), LastLoginAt = DateTime.UtcNow.AddMinutes(-45) },
                new User { Username = "mike.docker", Email = "mike.docker@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "Mike Docker", PhoneNumber = "+1-555-0501", Department = "Ship Operations", PortId = ports.First(p => p.Code == "USLGB").PortId, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-100), LastLoginAt = DateTime.UtcNow.AddHours(-6) },
                new User { Username = "anna.report", Email = "anna.report@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "Anna Report", PhoneNumber = "+1-555-0601", Department = "Analytics", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-80), LastLoginAt = DateTime.UtcNow.AddDays(-1) }
            };

            // Insert only missing users by unique email (idempotent)
            var desiredEmails = desiredUsers.Select(u => u.Email).ToList();
            var existingUsers = await _context.Users.Where(u => desiredEmails.Contains(u.Email)).ToListAsync();
            var existingEmails = new HashSet<string>(existingUsers.Select(u => u.Email));
            var newUsers = desiredUsers.Where(u => !existingEmails.Contains(u.Email)).ToList();

            if (newUsers.Count > 0)
            {
                _context.Users.AddRange(newUsers);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} new users", newUsers.Count);
            }
            else
            {
                _logger.LogInformation("No new users to insert (already up-to-date)");
            }

            // Ensure required role assignments exist for all target users
            var allTargetUsers = await _context.Users.Where(u => desiredEmails.Contains(u.Email)).ToListAsync();
            var emailToUserId = allTargetUsers.ToDictionary(u => u.Email, u => u.UserId);

            var roleAssignments = new List<(string Email, int RoleId, DateTime AssignedAt)>
            {
                ("admin@containertrack.com", adminRole.RoleId, DateTime.UtcNow.AddDays(-365)),
                ("john.harbor@containertrack.com", portManagerRole.RoleId, DateTime.UtcNow.AddDays(-200)),
                ("maria.santos@containertrack.com", portManagerRole.RoleId, DateTime.UtcNow.AddDays(-180)),
                ("chen.wei@containertrack.com", portManagerRole.RoleId, DateTime.UtcNow.AddDays(-150)),
                ("sarah.crane@containertrack.com", operatorRole.RoleId, DateTime.UtcNow.AddDays(-120)),
                ("mike.docker@containertrack.com", operatorRole.RoleId, DateTime.UtcNow.AddDays(-100)),
                ("anna.report@containertrack.com", viewerRole.RoleId, DateTime.UtcNow.AddDays(-80))
            };

            var existingUserRoles = await _context.UserRoles.AsNoTracking().ToListAsync();
            var existingPairs = new HashSet<(int UserId, int RoleId)>(existingUserRoles.Select(ur => (ur.UserId, ur.RoleId)));

            var userRolesToAdd = new List<UserRole>();
            foreach (var (email, roleId, assignedAt) in roleAssignments)
            {
                if (emailToUserId.TryGetValue(email, out var userId))
                {
                    if (!existingPairs.Contains((userId, roleId)))
                    {
                        userRolesToAdd.Add(new UserRole { UserId = userId, RoleId = roleId, AssignedAt = assignedAt });
                    }
                }
            }

            if (userRolesToAdd.Count > 0)
            {
                _context.UserRoles.AddRange(userRolesToAdd);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} new user-role assignments", userRolesToAdd.Count);
            }
            else
            {
                _logger.LogInformation("No new user-role assignments to insert (already up-to-date)");
            }
        }

        private Task SeedShippingLinesAsync()
        {
            _logger.LogInformation("Seeding shipping lines...");
            // This would be a new entity if you want to track shipping companies
            // For now, we'll include this data in the ship names
            return Task.CompletedTask;
        }

        private async Task SeedShipsAsync()
        {
            _logger.LogInformation("Seeding ships...");

            var ports = await _context.Ports.ToListAsync();
            var shipTypes = new[] { "Container Ship", "Bulk Carrier", "Tanker", "Car Carrier", "General Cargo" };
            var flags = new[] { "Panama", "Liberia", "Marshall Islands", "Hong Kong", "Singapore", "Malta", "Bahamas" };
            var statuses = new[] { "At Sea", "Docked", "Loading", "Unloading", "Maintenance", "Approaching" };

            var realShips = new List<(string Name, string IMO, int Capacity, string Flag, string Type)>
            {
                // Maersk Line
                ("Maersk Madrid", "IMO9778475", 20568, "Denmark", "Container Ship"),
                ("Maersk Milano", "IMO9778487", 20568, "Denmark", "Container Ship"),
                ("Maersk Munich", "IMO9778499", 20568, "Denmark", "Container Ship"),
                ("Maersk Mc-Kinney Moller", "IMO9619907", 18270, "Denmark", "Container Ship"),
                ("Maersk Edinburgh", "IMO9778474", 14000, "Denmark", "Container Ship"),

                // MSC (Mediterranean Shipping Company)
                ("MSC Gülsün", "IMO9811000", 23756, "Panama", "Container Ship"),
                ("MSC Mina", "IMO9811012", 23756, "Panama", "Container Ship"),
                ("MSC Sixin", "IMO9811024", 23756, "Panama", "Container Ship"),
                ("MSC Virtuosa", "IMO9803613", 19224, "Malta", "Container Ship"),
                ("MSC Grandiosa", "IMO9803625", 19224, "Malta", "Container Ship"),

                // COSCO Shipping
                ("COSCO Shipping Universe", "IMO9795815", 21237, "China", "Container Ship"),
                ("COSCO Shipping Galaxy", "IMO9795827", 21237, "China", "Container Ship"),
                ("COSCO Shipping Solar", "IMO9795839", 21237, "China", "Container Ship"),
                ("COSCO Development", "IMO9247455", 19100, "Hong Kong", "Container Ship"),
                ("COSCO Oceania", "IMO9247467", 19100, "Hong Kong", "Container Ship"),

                // Evergreen Marine
                ("Ever Ace", "IMO9811859", 23992, "Panama", "Container Ship"),
                ("Ever Apex", "IMO9811861", 23992, "Panama", "Container Ship"),
                ("Ever Arsenal", "IMO9811873", 23992, "Panama", "Container Ship"),
                ("Ever Given", "IMO9811000", 20124, "Panama", "Container Ship"),
                ("Ever Globe", "IMO9811012", 20124, "Panama", "Container Ship"),

                // CMA CGM
                ("CMA CGM Antoine de Saint Exupery", "IMO9454436", 20776, "France", "Container Ship"),
                ("CMA CGM Jacques Saade", "IMO9454448", 20776, "France", "Container Ship"),
                ("CMA CGM Champs Elysees", "IMO9454450", 20776, "France", "Container Ship"),
                ("CMA CGM Bougainville", "IMO9300185", 17722, "Malta", "Container Ship"),

                // OOCL (Orient Overseas Container Line)
                ("OOCL Hong Kong", "IMO9633565", 21413, "Hong Kong", "Container Ship"),
                ("OOCL Germany", "IMO9633577", 21413, "Hong Kong", "Container Ship"),
                ("OOCL Japan", "IMO9633589", 21413, "Hong Kong", "Container Ship"),
                ("OOCL United Kingdom", "IMO9633591", 21413, "Hong Kong", "Container Ship"),

                // HMM (Hyundai Merchant Marine)
                ("HMM Algeciras", "IMO9795803", 23964, "South Korea", "Container Ship"),
                ("HMM Oslo", "IMO9795815", 23964, "South Korea", "Container Ship"),
                ("HMM Rotterdam", "IMO9795827", 23964, "South Korea", "Container Ship"),
                ("HMM Southampton", "IMO9795839", 23964, "South Korea", "Container Ship"),

                // Yang Ming Marine Transport
                ("Yang Ming Wisdom", "IMO9454462", 20170, "Taiwan", "Container Ship"),
                ("Yang Ming Worth", "IMO9454474", 20170, "Taiwan", "Container Ship"),

                // Hapag-Lloyd
                ("Hapag-Lloyd Berlin", "IMO9811885", 23500, "Germany", "Container Ship"),
                ("Hapag-Lloyd Tanjong Pagar", "IMO9811897", 23500, "Germany", "Container Ship"),

                // ONE (Ocean Network Express)
                ("ONE Innovation", "IMO9811909", 20170, "Japan", "Container Ship"),
                ("ONE Inspiration", "IMO9811911", 20170, "Japan", "Container Ship"),

                // Bulk Carriers and Other Vessels
                ("Vale Beijing", "IMO9628683", 400000, "Marshall Islands", "Bulk Carrier"),
                ("Berge Everest", "IMO9593845", 388000, "Marshall Islands", "Bulk Carrier"),
                ("TI Europe", "IMO9248731", 441893, "Belgium", "Tanker"),
                ("Seawise Giant", "IMO7381154", 564763, "Singapore", "Tanker"),

                // Car Carriers
                ("Höegh Target", "IMO9778501", 8500, "Norway", "Car Carrier"),
                ("Morning Christina", "IMO9778513", 8500, "Panama", "Car Carrier")
            };

            var ships = new List<Ship>();

            foreach (var (name, imo, capacity, flag, type) in realShips)
            {
                var ship = new Ship
                {
                    Name = name,
                    ImoNumber = imo,
                    Flag = flag,
                    Type = type,
                    Capacity = capacity,
                    Status = statuses[_random.Next(statuses.Length)],
                    Length = type == "Container Ship" ? 
                        capacity > 20000 ? 400m : 
                        capacity > 15000 ? 366m : 
                        capacity > 10000 ? 334m : 294m : 
                        _random.Next(200, 450),
                    Beam = type == "Container Ship" ? 
                        capacity > 20000 ? 61m : 
                        capacity > 15000 ? 51m : 
                        capacity > 10000 ? 45m : 40m : 
                        _random.Next(32, 65),
                    Draft = _random.Next(12, 18),
                    GrossTonnage = capacity * (_random.Next(15, 25) / 10m),
                    YearBuilt = _random.Next(2010, 2024),
                    Speed = _random.Next(18, 25),
                    Heading = _random.Next(0, 360),
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 730)),
                    UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 24))
                };

                // Assign to port if docked
                if (ship.Status == "Docked" || ship.Status == "Loading" || ship.Status == "Unloading")
                {
                    ship.CurrentPortId = ports[_random.Next(ports.Count)].PortId;
                    // Ensure next port is set even when currently docked/loading/unloading
                    ship.NextPort = ports[_random.Next(ports.Count)].Name;
                }

                // Set coordinates and next port for ships at sea
                if (ship.Status == "At Sea" || ship.Status == "Approaching")
                {
                    ship.Coordinates = GenerateRealisticCoordinates();
                    ship.NextPort = ports[_random.Next(ports.Count)].Name;
                    ship.EstimatedArrival = DateTime.UtcNow.AddHours(_random.Next(6, 168)); // 6 hours to 7 days
                }

                // Ensure coordinates are always set to satisfy NOT NULL constraint
                if (string.IsNullOrWhiteSpace(ship.Coordinates))
                {
                    ship.Coordinates = GenerateRealisticCoordinates();
                }

                // Ensure NextPort is always set to satisfy NOT NULL constraint
                if (string.IsNullOrWhiteSpace(ship.NextPort))
                {
                    ship.NextPort = ports[_random.Next(ports.Count)].Name;
                }

                ships.Add(ship);
            }

            _context.Ships.AddRange(ships);
            await _context.SaveChangesAsync();
        }

        private async Task SeedBerthsAsync()
        {
            _logger.LogInformation("Seeding berths...");

            var ports = await _context.Ports.ToListAsync();
            var berths = new List<Berth>();
            var berthTypes = new[] { "Container", "Bulk", "RoRo", "Tanker", "General Cargo", "Cruise" };
            var statuses = new[] { "Available", "Occupied", "Maintenance", "Under Construction" };

            foreach (var port in ports)
            {
                var berthCount = port.TotalContainerCapacity switch
                {
                    > 30000000 => _random.Next(25, 35),  // Mega ports
                    > 15000000 => _random.Next(18, 25),  // Large ports
                    > 5000000 => _random.Next(12, 18),   // Medium ports
                    _ => _random.Next(6, 12)             // Smaller ports
                };

                for (int i = 1; i <= berthCount; i++)
                {
                    var berthType = berthTypes[_random.Next(berthTypes.Length)];
                    var berth = new Berth
                    {
                        Name = $"{port.Code}-B{i:D2}",
                        Identifier = $"{port.Code}-B{i:D2}",
                        PortId = port.PortId,
                        Type = berthType,
                        Status = statuses[_random.Next(statuses.Length)],
                        Capacity = berthType == "Container" ? _random.Next(300, 800) : _random.Next(50, 200),
                        CurrentLoad = 0, // Will be updated when we create assignments
                        MaxShipLength = _random.Next(200, 450),
                        MaxDraft = _random.Next(12, 20),
                        AvailableServices = berthType == "Container" ? "Crane, Refueling, Maintenance" : "Refueling, Maintenance",
                        Priority = "Medium",
                        Notes = string.Empty,
                        CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(365, 1825)),
                        UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 168))
                    };

                    berths.Add(berth);
                }
            }

            _context.Berths.AddRange(berths);
            await _context.SaveChangesAsync();
        }

        private async Task SeedContainerTypesAsync()
        {
            _logger.LogInformation("Seeding container types...");
            // Container types are embedded in the container seeding
        }

        private async Task SeedContainersAsync()
        {
            _logger.LogInformation("Seeding containers...");

            var ships = await _context.Ships.ToListAsync();
            var ports = await _context.Ports.ToListAsync();
            
            var containerTypes = new[] 
            { 
                "Dry", "Refrigerated", "Open Top", "Flat Rack", "Tank", 
                "Bulk", "High Cube", "Platform" 
            };
            
            var cargoTypes = new[] 
            { 
                "Electronics", "Textiles", "Automotive Parts", "Machinery", "Food Products",
                "Chemicals", "Raw Materials", "Consumer Goods", "Pharmaceuticals", "Oil",
                "Grain", "Coal", "Steel", "Furniture", "Toys", "Clothing"
            };

            // Enhanced cargo descriptions mapped to cargo types
            var cargoDescriptions = new Dictionary<string, string[]>
            {
                ["Electronics"] = new[] { "Smartphones and tablets", "Laptop computers", "Television sets", "Audio equipment", "Gaming consoles", "Electronic components", "Semiconductor devices" },
                ["Textiles"] = new[] { "Cotton fabric rolls", "Synthetic yarn", "Finished garments", "Raw cotton bales", "Polyester fabrics", "Denim clothing", "Home textiles" },
                ["Automotive Parts"] = new[] { "Engine components", "Brake systems", "Transmission parts", "Electrical harnesses", "Tires and wheels", "Body panels", "Interior components" },
                ["Machinery"] = new[] { "Industrial pumps", "Construction equipment", "Manufacturing tools", "Heavy machinery parts", "Agricultural equipment", "Mining machinery", "Power generators" },
                ["Food Products"] = new[] { "Frozen seafood", "Canned goods", "Coffee beans", "Rice and grains", "Dairy products", "Processed meat", "Fresh fruits", "Vegetable oil" },
                ["Chemicals"] = new[] { "Industrial solvents", "Fertilizers", "Plastic resins", "Paint and coatings", "Pharmaceutical ingredients", "Cleaning agents", "Agricultural chemicals" },
                ["Raw Materials"] = new[] { "Iron ore", "Copper concentrate", "Aluminum ingots", "Timber logs", "Natural rubber", "Mineral ores", "Scrap metal" },
                ["Consumer Goods"] = new[] { "Household appliances", "Sporting goods", "Beauty products", "Kitchen utensils", "Office supplies", "Personal care items", "Home decorations" },
                ["Pharmaceuticals"] = new[] { "Prescription medications", "Medical devices", "Vaccine supplies", "Laboratory equipment", "Surgical instruments", "Diagnostic kits", "Health supplements" },
                ["Oil"] = new[] { "Crude oil", "Refined petroleum", "Lubricating oil", "Diesel fuel", "Gasoline", "Heating oil", "Industrial oil" },
                ["Grain"] = new[] { "Wheat shipment", "Corn kernels", "Soybeans", "Rice cargo", "Barley grain", "Oats shipment", "Agricultural feed" },
                ["Coal"] = new[] { "Thermal coal", "Metallurgical coal", "Steam coal", "Anthracite coal", "Lignite coal", "Coal briquettes", "Coke fuel" },
                ["Steel"] = new[] { "Steel coils", "Steel pipes", "Structural steel", "Steel plates", "Reinforcement bars", "Steel wire", "Galvanized steel" },
                ["Furniture"] = new[] { "Office furniture", "Home furniture sets", "Wooden chairs", "Metal desks", "Modular furniture", "Outdoor furniture", "Children's furniture" },
                ["Toys"] = new[] { "Plastic toys", "Educational toys", "Electronic games", "Stuffed animals", "Building blocks", "Action figures", "Board games" },
                ["Clothing"] = new[] { "Men's apparel", "Women's fashion", "Children's clothing", "Sports wear", "Winter jackets", "Casual wear", "Formal attire" }
            };
            
            var statuses = new[] 
            { 
                "Available", "In Transit", "Loading", "Loaded", "Unloading", 
                "At Port", "In Storage", "Maintenance", "Customs Hold" 
            };

            var conditions = new[] { "Excellent", "Good", "Fair", "Damaged", "Under Repair" };

            var containers = new List<Container>();
            var containerPrefixes = new[] { "MAEU", "MSCU", "COSU", "EGLV", "CMAU", "OOLU", "HJMU", "YMLU", "HPLU", "ONEY" };

            for (int i = 1; i <= 100; i++)
            {
                var prefix = containerPrefixes[_random.Next(containerPrefixes.Length)];
                var containerNumber = $"{prefix}{_random.Next(1000000, 9999999)}";
                var cargoType = cargoTypes[_random.Next(cargoTypes.Length)];
                var containerType = containerTypes[_random.Next(containerTypes.Length)];
                var status = statuses[_random.Next(statuses.Length)];
                var condition = conditions[_random.Next(conditions.Length)];
                
                // Get realistic cargo description based on cargo type
                var possibleDescriptions = cargoDescriptions.ContainsKey(cargoType) 
                    ? cargoDescriptions[cargoType] 
                    : new[] { $"Various {cargoType.ToLower()}" };
                var cargoDescription = possibleDescriptions[_random.Next(possibleDescriptions.Length)];

                // Select random origin and destination ports
                var originPort = ports[_random.Next(ports.Count)];
                var destinationPort = ports[_random.Next(ports.Count)];
                
                // Ensure origin and destination are different
                while (destinationPort.PortId == originPort.PortId)
                {
                    destinationPort = ports[_random.Next(ports.Count)];
                }

                var container = new Container
                {
                    ContainerId = containerNumber,
                    CargoType = cargoType,
                    CargoDescription = cargoDescription,
                    Type = containerType,
                    Status = status,
                    Condition = condition,
                    Weight = _random.Next(5000, 28000), // Realistic container weights in kg
                    Size = _random.Next(10) < 7 ? "40ft" : "20ft", // 70% are 40ft containers
                    CurrentLocation = ports[_random.Next(ports.Count)].Name,
                    Destination = destinationPort.Name,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                    UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 72))
                };

                // Assign some containers to ships
                if (_random.Next(10) < 3 && ships.Any()) // 30% of containers are on ships
                {
                    container.ShipId = ships[_random.Next(ships.Count)].ShipId;
                    container.Status = "In Transit"; // Override status for containers on ships
                }

                // Set temperature for refrigerated containers
                if (container.Type == "Refrigerated")
                {
                    container.Temperature = _random.Next(-25, 15); // Typical reefer temps in Celsius
                }

                // Set estimated arrival for containers in transit
                if (container.Status == "In Transit")
                {
                    container.EstimatedArrival = DateTime.UtcNow.AddHours(_random.Next(6, 336)); // 6 hours to 2 weeks
                }

                containers.Add(container);
            }

            _context.Containers.AddRange(containers);
            await _context.SaveChangesAsync();
        }

        private async Task SeedBerthAssignmentsAsync()
        {
            _logger.LogInformation("Seeding berth assignments...");

            var berths = await _context.Berths.Include(b => b.Port).ToListAsync();
            var ships = await _context.Ships.ToListAsync();
            var users = await _context.Users.ToListAsync();

            var assignments = new List<BerthAssignment>();
            var assignmentTypes = new[] { "Loading", "Unloading", "Both", "Maintenance", "Bunker" };
            var statuses = new[] { "Scheduled", "Active", "Completed", "Cancelled" };

            // Create assignments for occupied berths
            var occupiedBerths = berths.Where(b => b.Status == "Occupied").ToList();
            
            foreach (var berth in occupiedBerths.Take(Math.Min(occupiedBerths.Count, ships.Count)))
            {
                var ship = ships[_random.Next(ships.Count)];
                var user = users.FirstOrDefault(u => u.PortId == berth.PortId) ?? users.First();
                
                var assignment = new BerthAssignment
                {
                    BerthId = berth.BerthId,
                    ShipId = ship.ShipId,
                    AssignmentType = assignmentTypes[_random.Next(assignmentTypes.Length)],
                    Status = statuses[_random.Next(statuses.Length)],
                    Priority = _random.Next(10) < 3 ? "High" : _random.Next(10) < 6 ? "Medium" : "Low",
                    ScheduledArrival = DateTime.UtcNow.AddHours(-_random.Next(1, 48)),
                    ScheduledDeparture = DateTime.UtcNow.AddHours(_random.Next(6, 72)),
                    AssignedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 72)),
                    CreatedByUserId = user.UserId,
                    Notes = string.Empty,
                    CreatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 72)),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-_random.Next(1, 60))
                };

                // Set actual arrival for active/completed assignments
                if (assignment.Status == "Active" || assignment.Status == "Completed")
                {
                    if (assignment.ScheduledArrival.HasValue)
                    {
                        assignment.ActualArrival = assignment.ScheduledArrival.Value.AddMinutes(_random.Next(-30, 120));
                    }
                }

                // Set actual departure for completed assignments
                if (assignment.Status == "Completed")
                {
                    assignment.ActualDeparture = assignment.ActualArrival?.AddHours(_random.Next(6, 48));
                }

                assignments.Add(assignment);
                
                // Update berth load
                berth.CurrentLoad = _random.Next(berth.Capacity / 2, berth.Capacity);
            }

            _context.BerthAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();
        }

        private async Task SeedShipContainerOperationsAsync()
        {
            _logger.LogInformation("Seeding ship container operations...");

            var ships = await _context.Ships.Include(s => s.Containers).ToListAsync();
            var containers = await _context.Containers.Where(c => c.ShipId.HasValue).ToListAsync();

            var operations = new List<ShipContainer>();
            var statuses = new[] { "Loaded", "Loading", "Unloading", "Planned" };

            foreach (var container in containers)
            {
                if (container.ShipId.HasValue)
                {
                    var ship = ships.First(s => s.ShipId == container.ShipId.Value);
                    
                    var operation = new ShipContainer
                    {
                        ShipId = ship.ShipId,
                        ContainerId = container.ContainerId,
                        LoadedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 168))
                    };

                    // Set unloaded time for completed operations
                    // ShipContainer model does not track unload info; keep only required fields

                    operations.Add(operation);
                }
            }

            _context.ShipContainers.AddRange(operations);
            await _context.SaveChangesAsync();
        }

        private async Task SeedContainerMovementsAsync()
        {
            _logger.LogInformation("Seeding container movements...");

            // Check if movements already exist
            var existingMovementsCount = await _context.ContainerMovements.CountAsync();
            if (existingMovementsCount > 0)
            {
                _logger.LogInformation("Container movements already exist ({Count} found), skipping seeding", existingMovementsCount);
                return;
            }

            var containers = await _context.Containers.Take(100).ToListAsync(); // Sample movements for first 100 containers
            var ports = await _context.Ports.ToListAsync();
            var berths = await _context.Berths.ToListAsync();
            var ships = await _context.Ships.ToListAsync();
            var users = await _context.Users.ToListAsync();

            if (!containers.Any() || !ports.Any() || !users.Any())
            {
                _logger.LogWarning("Required data not found for container movements seeding (containers: {ContainerCount}, ports: {PortCount}, users: {UserCount})", 
                    containers.Count, ports.Count, users.Count);
                return;
            }

            var movements = new List<ContainerMovement>();
            var movementTypes = new[] { "Loading", "Unloading", "Transfer", "Storage", "Inspection", "Customs" };

            foreach (var container in containers)
            {
                // Create 2-5 movement records per container
                var movementCount = _random.Next(2, 6);
                
                for (int i = 0; i < movementCount; i++)
                {
                    var movement = new ContainerMovement
                    {
                        ContainerId = container.ContainerId,
                        MovementType = movementTypes[_random.Next(movementTypes.Length)],
                        FromLocation = i == 0 ? "Origin Port" : $"Location {i}",
                        ToLocation = i == movementCount - 1 ? container.CurrentLocation : $"Location {i + 1}",
                        MovementTimestamp = container.CreatedAt.AddHours(i * _random.Next(6, 48)),
                        Status = i == movementCount - 1 ? "In Progress" : "Completed",
                        Coordinates = GenerateRealisticCoordinates(), // REQUIRED: Generate coordinates
                        Notes = $"Movement {i + 1} of {movementCount} - {movementTypes[_random.Next(movementTypes.Length)]}", // REQUIRED: Add notes
                        PortId = ports[_random.Next(ports.Count)].PortId,
                        RecordedByUserId = users[_random.Next(users.Count)].UserId,
                        CreatedAt = container.CreatedAt.AddHours(i * _random.Next(6, 48)),
                        ActualCompletion = i == movementCount - 1 ? null : container.CreatedAt.AddHours(i * _random.Next(6, 48)).AddHours(_random.Next(1, 12)),
                        EstimatedCompletion = container.CreatedAt.AddHours(i * _random.Next(6, 48)).AddHours(_random.Next(2, 24))
                    };

                    // Add temperature for refrigerated containers
                    if (container.Type == "Refrigerated")
                    {
                        movement.Temperature = _random.Next(-25, 15);
                        movement.Humidity = _random.Next(40, 85);
                    }

                    // Add berth and ship info for some movements
                    if (_random.Next(10) < 4)
                    {
                        movement.BerthId = berths[_random.Next(berths.Count)].BerthId;
                    }

                    if (_random.Next(10) < 3)
                    {
                        movement.ShipId = ships[_random.Next(ships.Count)].ShipId;
                    }

                    movements.Add(movement);
                }
            }

            if (movements.Count > 0)
            {
                _context.ContainerMovements.AddRange(movements);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} container movements successfully", movements.Count);
            }
            else
            {
                _logger.LogInformation("No container movements to seed");
            }
        }

        private async Task SeedEventsAsync()
        {
            _logger.LogInformation("Seeding events...");

            var containers = await _context.Containers.Take(20).ToListAsync();
            var ships = await _context.Ships.Take(15).ToListAsync();
            var berths = await _context.Berths.Take(25).ToListAsync();
            var ports = await _context.Ports.Take(10).ToListAsync();
            var users = await _context.Users.ToListAsync();

            var events = new List<Event>();
            var eventTypes = new[] 
            { 
                "Ship Arrival", "Ship Departure", "Container Loading", "Container Unloading",
                "Berth Assignment", "Equipment Failure", "Security Alert", "Weather Warning",
                "Customs Inspection", "Maintenance Scheduled", "Delay Notification", "Cargo Damage"
            };
            
            var priorities = new[] { "Low", "Medium", "High", "Critical" };
            var severities = new[] { "Info", "Warning", "Error", "Critical" };

            for (int i = 0; i < 40; i++)
            {
                var eventType = eventTypes[_random.Next(eventTypes.Length)];
                var priority = priorities[_random.Next(priorities.Length)];
                var severity = severities[_random.Next(severities.Length)];
                
                var eventObj = new Event
                {
                    EventType = eventType,
                    Title = GenerateEventTitle(eventType),
                    Description = GenerateEventDescription(eventType),
                    Priority = priority,
                    Severity = severity,
                    Status = _random.Next(10) < 7 ? "New" : _random.Next(10) < 5 ? "Acknowledged" : "Resolved",
                    RequiresAction = priority == "High" || priority == "Critical",
                    EventTimestamp = DateTime.UtcNow.AddHours(-_random.Next(1, 168)),
                    CreatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 168))
                };

                // Assign to entities based on event type
                switch (eventType)
                {
                    case "Container Loading":
                    case "Container Unloading":
                    case "Cargo Damage":
                        if (containers.Any())
                            eventObj.ContainerId = containers[_random.Next(containers.Count)].ContainerId;
                        break;
                        
                    case "Ship Arrival":
                    case "Ship Departure":
                        if (ships.Any())
                            eventObj.ShipId = ships[_random.Next(ships.Count)].ShipId;
                        break;
                        
                    case "Berth Assignment":
                    case "Equipment Failure":
                        if (berths.Any())
                            eventObj.BerthId = berths[_random.Next(berths.Count)].BerthId;
                        break;
                        
                    default:
                        if (ports.Any())
                            eventObj.PortId = ports[_random.Next(ports.Count)].PortId;
                        break;
                }

                // Assign to user if requires action
                if (eventObj.RequiresAction && users.Any())
                {
                    eventObj.AssignedToUserId = users[_random.Next(users.Count)].UserId;
                }

                events.Add(eventObj);
            }

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAnalyticsDataAsync()
        {
            _logger.LogInformation("Seeding analytics data...");

            var ports = await _context.Ports.ToListAsync();
            var berths = await _context.Berths.ToListAsync();
            var ships = await _context.Ships.ToListAsync();

            var analytics = new List<Analytics>();
            var metricTypes = new[] 
            { 
                "Throughput", "Utilization", "Turnaround Time", "Efficiency", 
                "Revenue", "Costs", "Delays", "Performance Score" 
            };
            var periods = new[] { "Hour", "Day", "Week", "Month", "Quarter", "Year" };

            // Generate historical analytics data for the past 12 months
            var startDate = DateTime.UtcNow.AddMonths(-12);
            
            foreach (var port in ports.Take(10)) // Top 10 ports
            {
                for (int month = 0; month < 12; month++)
                {
                    var date = startDate.AddMonths(month);
                    
                    foreach (var metricType in metricTypes)
                    {
                        var analytic = new Analytics
                        {
                            MetricType = metricType,
                            Value = 95.7m,
                            Period = "Month",
                            MetricTimestamp = date,
                            PortId = port.PortId,
                            CreatedAt = date.AddDays(_random.Next(0, 30))
                        };

                        analytics.Add(analytic);
                    }
                }
            }

            // Generate berth-specific analytics
            foreach (var berth in berths.Take(50))
            {
                for (int week = 0; week < 12; week++)
                {
                    var date = DateTime.UtcNow.AddDays(-7);
                    
                    var berthAnalytic = new Analytics
                    {
                        MetricType = "Utilization",
                        Value = _random.Next(40, 95),
                        Period = "Week",
                        MetricTimestamp = date,
                        BerthId = berth.BerthId,
                        PortId = berth.PortId,
                        CreatedAt = date
                    };

                    analytics.Add(berthAnalytic);
                }
            }

            _context.Analytics.AddRange(analytics);
            await _context.SaveChangesAsync();
        }

        // Helper methods
        private string HashPassword(string password)
        {
            // SECURITY: Use proper password hashing with salt (same as AuthService)
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(new User(), password);
        }

        private string GenerateRealisticCoordinates()
        {
            // Generate coordinates for major shipping routes
            var routes = new[]
            {
                (35.6762, 139.6503), // Tokyo Bay
                (22.3193, 114.1694), // Hong Kong
                (1.3521, 103.8198),  // Singapore
                (51.8985, 4.4134),   // Rotterdam
                (33.7490, -118.2437) // Los Angeles
            };

            var route = routes[_random.Next(routes.Length)];
            var lat = route.Item1 + (_random.NextDouble() - 0.5) * 10; // ±5 degrees variation
            var lng = route.Item2 + (_random.NextDouble() - 0.5) * 20; // ±10 degrees variation

            return $"{lat:F4},{lng:F4}";
        }

        private string GenerateContainerPosition()
        {
            var sections = new[] { "Deck", "Hold" };
            var section = sections[_random.Next(sections.Length)];
            var bay = _random.Next(1, 21).ToString("D2");
            var row = _random.Next(1, 9).ToString("D2");
            var tier = _random.Next(1, 9).ToString("D2");

            return $"{section} {bay}-{row}-{tier}";
        }

        private string GenerateEventTitle(string eventType)
        {
            return eventType switch
            {
                "Ship Arrival" => $"Ship arriving at berth",
                "Ship Departure" => $"Ship departing from port",
                "Container Loading" => $"Container loading operation",
                "Container Unloading" => $"Container unloading operation",
                "Berth Assignment" => $"New berth assignment",
                "Equipment Failure" => $"Equipment malfunction reported",
                "Security Alert" => $"Security incident detected",
                "Weather Warning" => $"Adverse weather conditions",
                "Customs Inspection" => $"Customs inspection required",
                "Maintenance Scheduled" => $"Scheduled maintenance",
                "Delay Notification" => $"Operation delay reported",
                "Cargo Damage" => $"Cargo damage incident",
                _ => $"System event: {eventType}"
            };
        }

        private string GenerateEventDescription(string eventType)
        {
            return eventType switch
            {
                "Ship Arrival" => "Vessel has arrived and is requesting berth assignment for container operations.",
                "Ship Departure" => "Vessel has completed operations and is departing the port area.",
                "Container Loading" => "Container loading operation is in progress. Estimated completion in 2 hours.",
                "Container Unloading" => "Container unloading operation has begun. Please ensure clear access routes.",
                "Berth Assignment" => "New berth assignment has been created. Please coordinate with vessel operations.",
                "Equipment Failure" => "Port equipment has experienced a malfunction. Maintenance team has been notified.",
                "Security Alert" => "Security incident requires immediate attention from authorized personnel.",
                "Weather Warning" => "Weather conditions may affect port operations. Monitor situation closely.",
                "Customs Inspection" => "Container requires customs inspection before release. Hold until cleared.",
                "Maintenance Scheduled" => "Scheduled maintenance will affect operations. Plan accordingly.",
                "Delay Notification" => "Operation is experiencing delays due to unforeseen circumstances.",
                "Cargo Damage" => "Cargo damage has been reported. Initiate damage assessment procedure.",
                _ => $"System generated event for {eventType}. Please review and take appropriate action."
            };
        }

        private decimal GenerateRealisticMetricValue(string metricType)
        {
            return metricType switch
            {
                "Throughput" => _random.Next(50000, 500000), // TEUs per month
                "Utilization" => _random.Next(60, 95), // Percentage
                "Turnaround Time" => _random.Next(12, 48), // Hours
                "Efficiency" => _random.Next(75, 98), // Percentage
                "Revenue" => _random.Next(1000000, 50000000), // USD
                "Costs" => _random.Next(500000, 25000000), // USD
                "Delays" => _random.Next(5, 25), // Percentage
                "Performance Score" => _random.Next(70, 95), // Score
                _ => _random.Next(1, 100)
            };
        }
    }
}

