using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services
{
    /// <summary>
    /// Comprehensive data seeding service with realistic maritime industry data
    /// </summary>
    public class ComprehensiveDataSeedingService(
        ApplicationDbContext context,
        ILogger<ComprehensiveDataSeedingService> logger)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ComprehensiveDataSeedingService> _logger = logger;
        private readonly Random _random = new();

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
            _logger.LogInformation("Seeding lean world ports...");

            var ports = new List<Port>
            {
                // Strategic global coverage (5 major regions)
                new Port { Name = "Port of Singapore", Code = "SGSIN", Country = "Singapore", Location = "Singapore", TotalContainerCapacity = 37200000, CurrentContainerCount = 28000000, Status = "Operational", TimeZone = "Asia/Singapore" },
                new Port { Name = "Port of Rotterdam", Code = "NLRTM", Country = "Netherlands", Location = "Rotterdam, Netherlands", TotalContainerCapacity = 14800000, CurrentContainerCount = 11100000, Status = "Operational", TimeZone = "Europe/Amsterdam" },
                new Port { Name = "Port of Los Angeles", Code = "USLAX", Country = "United States", Location = "Los Angeles, CA, USA", TotalContainerCapacity = 9200000, CurrentContainerCount = 6900000, Status = "Operational", TimeZone = "America/Los_Angeles" },
                new Port { Name = "Port of Shanghai", Code = "CNSHA", Country = "China", Location = "Shanghai, China", TotalContainerCapacity = 47000000, CurrentContainerCount = 35000000, Status = "Operational", TimeZone = "Asia/Shanghai" },
                new Port { Name = "Port of Dubai", Code = "AEDXB", Country = "United Arab Emirates", Location = "Dubai, UAE", TotalContainerCapacity = 15000000, CurrentContainerCount = 11200000, Status = "Operational", TimeZone = "Asia/Dubai" },
                new Port { Name = "Port of Santos", Code = "BRSSZ", Country = "Brazil", Location = "Santos, Brazil", TotalContainerCapacity = 4200000, CurrentContainerCount = 3100000, Status = "Operational", TimeZone = "America/Sao_Paulo" }
            };

            // Ensure required fields are populated
            foreach (var port in ports)
            {
                port.Coordinates = GenerateRealisticCoordinates();
                port.OperatingHours = "24/7";
                port.Services = "Container,Bulk,RoRo";
                port.ContactInfo = $"Operations Desk, {port.Name}";
                port.MaxShipCapacity = 25; // Reasonable for testing
                port.CurrentShipCount = _random.Next(5, 15);
                port.CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 365));
                port.UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 24));
            }

            var existingCodes = await _context.Ports.AsNoTracking().Select(p => p.Code).ToListAsync();
            var newPorts = ports.Where(p => !existingCodes.Contains(p.Code)).ToList();

            if (newPorts.Count > 0)
            {
                _context.Ports.AddRange(newPorts);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Inserted {Count} lean ports", newPorts.Count);
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
                new User { Username = "mike.docker", Email = "mike.docker@containertrack.com", PasswordHash = HashPassword(Environment.GetEnvironmentVariable("DEFAULT_USER_PASSWORD") ?? "TempPass123!ChangeMePlease"), FullName = "Mike Docker", PhoneNumber = "+1-555-0501", Department = "Ship Operations", PortId = ports.First(p => p.Code == "USLAX").PortId, IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-100), LastLoginAt = DateTime.UtcNow.AddHours(-6) },
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
            _logger.LogInformation("Seeding lean ships collection...");

            var ports = await _context.Ports.ToListAsync();
            var statuses = new[] { "At Sea", "Docked", "Loading", "Unloading", "Approaching" };

            // Curated list covering all major shipping lines (8 ships total)
            var leanShips = new List<(string Name, string IMO, int Capacity, string Flag, string Type)>
            {
                // Major shipping lines representation
                ("Maersk Madrid", "IMO9778475", 20568, "Denmark", "Container Ship"),
                ("MSC Gülsün", "IMO9811000", 23756, "Panama", "Container Ship"),
                ("COSCO Shipping Universe", "IMO9795815", 21237, "China", "Container Ship"),
                ("Ever Ace", "IMO9811859", 23992, "Panama", "Container Ship"),
                ("CMA CGM Jacques Saade", "IMO9454448", 20776, "France", "Container Ship"),
                ("OOCL Hong Kong", "IMO9633565", 21413, "Hong Kong", "Container Ship"),
                ("HMM Algeciras", "IMO9795803", 23964, "South Korea", "Container Ship"),
                ("Vale Beijing", "IMO9628683", 400000, "Marshall Islands", "Bulk Carrier"), // Different type
                ("TI Europe", "IMO9248731", 441893, "Belgium", "Tanker"), // Different type
                ("Höegh Target", "IMO9778501", 8500, "Norway", "Car Carrier") // Different type
            };

            var ships = new List<Ship>();

            foreach (var (name, imo, capacity, flag, type) in leanShips)
            {
                var ship = new Ship
                {
                    Name = name,
                    ImoNumber = imo,
                    Flag = flag,
                    Type = type,
                    Capacity = capacity,
                    Status = statuses[_random.Next(statuses.Length)],
                    Length = type == "Container Ship" ? 400m : _random.Next(200, 450),
                    Beam = type == "Container Ship" ? 59m : _random.Next(32, 65),
                    Draft = _random.Next(12, 18),
                    GrossTonnage = capacity * 1.5m,
                    YearBuilt = _random.Next(2015, 2024),
                    Speed = _random.Next(18, 25),
                    Heading = _random.Next(0, 360),
                    Coordinates = GenerateRealisticCoordinates(),
                    NextPort = ports[_random.Next(ports.Count)].Name,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(30, 365)),
                    UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 24))
                };

                // Assign to port if docked
                if (ship.Status == "Docked" || ship.Status == "Loading" || ship.Status == "Unloading")
                {
                    ship.CurrentPortId = ports[_random.Next(ports.Count)].PortId;
                }

                if (ship.Status == "At Sea" || ship.Status == "Approaching")
                {
                    ship.EstimatedArrival = DateTime.UtcNow.AddHours(_random.Next(6, 168));
                }

                ships.Add(ship);
            }

            _context.Ships.AddRange(ships);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} lean ships", ships.Count);
        }

        private async Task SeedBerthsAsync()
        {
            _logger.LogInformation("Seeding lean berths...");

            var ports = await _context.Ports.ToListAsync();
            var berths = new List<Berth>();
            var berthTypes = new[] { "Container", "Bulk", "RoRo", "Tanker", "General Cargo" };
            var statuses = new[] { "Available", "Occupied", "Maintenance" };

            foreach (var port in ports)
            {
                // 3-4 berths per port (total ~18-24 berths)
                var berthCount = 3;
                if (port.Code == "SGSIN" || port.Code == "CNSHA") berthCount = 4; // Larger ports get 4

                for (int i = 1; i <= berthCount; i++)
                {
                    var berth = new Berth
                    {
                        Name = $"{port.Code}-B{i:D2}",
                        Identifier = $"B{i:D2}",
                        PortId = port.PortId,
                        Type = berthTypes[i % berthTypes.Length], // Ensure all types covered
                        Status = statuses[_random.Next(statuses.Length)],
                        Capacity = _random.Next(300, 800),
                        CurrentLoad = 0,
                        MaxShipLength = _random.Next(300, 450),
                        MaxDraft = _random.Next(12, 20),
                        AvailableServices = "Crane,Refueling,Maintenance",
                        Priority = "Medium",
                        Notes = $"Berth {i} at {port.Name}",
                        CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(365, 1825)),
                        UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 168))
                    };

                    berths.Add(berth);
                }
            }

            _context.Berths.AddRange(berths);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} lean berths", berths.Count);
        }

        private async Task SeedContainersAsync()
        {
            _logger.LogInformation("Seeding lean containers...");

            var ships = await _context.Ships.ToListAsync();
            var ports = await _context.Ports.ToListAsync();
            
            // Ensure ALL container types and cargo types are represented
            var containerTypes = new[] { "Dry", "Refrigerated", "Open Top", "Flat Rack", "Tank", "Bulk", "High Cube", "Platform" };
            var cargoTypes = new[] { "Electronics", "Textiles", "Automotive Parts", "Machinery", "Food Products", "Chemicals", "Raw Materials", "Consumer Goods", "Pharmaceuticals" };
            var statuses = new[] { "Available", "In Transit", "Loading", "Loaded", "Unloading", "At Port", "In Storage", "Maintenance" };
            var conditions = new[] { "Excellent", "Good", "Fair", "Damaged" };

            var containers = new List<Container>();
            var containerPrefixes = new[] { "MAEU", "MSCU", "COSU", "EGLV", "CMAU" };

            // Create exactly 30 containers to cover all combinations systematically
            for (int i = 0; i < 30; i++)
            {
                var prefix = containerPrefixes[i % containerPrefixes.Length];
                var containerNumber = $"{prefix}{(2024000 + i):D7}";
                var containerType = containerTypes[i % containerTypes.Length]; // Cycle through all types
                var cargoType = cargoTypes[i % cargoTypes.Length]; // Cycle through all cargo types
                var status = statuses[i % statuses.Length]; // Cycle through all statuses
                var condition = conditions[i % conditions.Length]; // Cycle through all conditions

                var originPort = ports[i % ports.Count];
                var destinationPort = ports[(i + 1) % ports.Count]; // Different from origin

                var container = new Container
                {
                    ContainerId = containerNumber,
                    CargoType = cargoType,
                    CargoDescription = $"Sample {cargoType.ToLower()} shipment #{i + 1}",
                    Type = containerType,
                    Status = status,
                    Condition = condition,
                    Weight = _random.Next(8000, 26000), // Realistic range
                    Size = i % 3 == 0 ? "20ft" : "40ft", // Mix of sizes
                    CurrentLocation = originPort.Name,
                    Destination = destinationPort.Name,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 90)),
                    UpdatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 48))
                };

                // Assign some containers to ships (30% in transit)
                if (i % 10 < 3 && ships.Any())
                {
                    container.ShipId = ships[_random.Next(ships.Count)].ShipId;
                    container.Status = "In Transit";
                }

                // Set temperature for refrigerated containers
                if (container.Type == "Refrigerated")
                {
                    container.Temperature = _random.Next(-20, 10);
                }

                // Set estimated arrival for in-transit containers
                if (container.Status == "In Transit")
                {
                    container.EstimatedArrival = DateTime.UtcNow.AddHours(_random.Next(6, 168));
                }

                containers.Add(container);
            }

            _context.Containers.AddRange(containers);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} lean containers covering all types", containers.Count);
        }

        private async Task SeedBerthAssignmentsAsync()
        {
            _logger.LogInformation("Seeding lean berth assignments...");

            var berths = await _context.Berths.Include(b => b.Port).ToListAsync();
            var ships = await _context.Ships.ToListAsync();
            var users = await _context.Users.ToListAsync();

            var assignments = new List<BerthAssignment>();
            var assignmentTypes = new[] { "Loading", "Unloading", "Both", "Maintenance", "Bunker" };
            var statuses = new[] { "Scheduled", "Active", "Completed" };

            // Create 12-15 assignments (about 60-70% of berths occupied)
            var occupiedBerths = berths.Take(Math.Min(15, berths.Count)).ToList();
            
            for (int i = 0; i < occupiedBerths.Count && i < ships.Count; i++)
            {
                var berth = occupiedBerths[i];
                var ship = ships[i % ships.Count];
                var user = users.FirstOrDefault(u => u.PortId == berth.PortId) ?? users.First();
                
                var assignment = new BerthAssignment
                {
                    BerthId = berth.BerthId,
                    ShipId = ship.ShipId,
                    AssignmentType = assignmentTypes[i % assignmentTypes.Length],
                    Status = statuses[i % statuses.Length],
                    Priority = i % 5 == 0 ? "High" : i % 3 == 0 ? "Medium" : "Low",
                    ScheduledArrival = DateTime.UtcNow.AddHours(-_random.Next(1, 48)),
                    ScheduledDeparture = DateTime.UtcNow.AddHours(_random.Next(6, 72)),
                    AssignedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 72)),
                    CreatedByUserId = user.UserId,
                    Notes = $"Assignment {i + 1} for {ship.Name}",
                    CreatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 72)),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-_random.Next(1, 60))
                };

                // Set actual times for active/completed assignments
                if (assignment.Status == "Active" || assignment.Status == "Completed")
                {
                    assignment.ActualArrival = assignment.ScheduledArrival?.AddMinutes(_random.Next(-30, 60));
                }

                if (assignment.Status == "Completed")
                {
                    assignment.ActualDeparture = assignment.ActualArrival?.AddHours(_random.Next(8, 48));
                }

                assignments.Add(assignment);
                
                // Update berth status and load
                berth.Status = assignment.Status == "Active" ? "Occupied" : "Available";
                berth.CurrentLoad = assignment.Status == "Active" ? _random.Next(100, berth.Capacity) : 0;
            }

            _context.BerthAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} lean berth assignments", assignments.Count);
        }

        private async Task SeedContainerTypesAsync()
        {
            _logger.LogInformation("Seeding container types...");
            // This method can be empty if ContainerType is not a separate entity
            // Or implement if you have a ContainerType table
            return;
        }

        private async Task SeedShipContainerOperationsAsync()
        {
            _logger.LogInformation("Seeding ship container operations...");
            
            var ships = await _context.Ships.ToListAsync();
            var containers = await _context.Containers.Take(20).ToListAsync();
            
            if (!ships.Any() || !containers.Any())
            {
                _logger.LogWarning("Required data not found for ship container operations seeding");
                return;
            }

            // This would seed ShipContainer junction table if it exists
            // For now, we'll just log completion
            _logger.LogInformation("Ship container operations seeding completed");
        }

        private async Task SeedContainerMovementsAsync()
        {
            _logger.LogInformation("Seeding lean container movements...");

            var containers = await _context.Containers.Take(15).ToListAsync(); // Sample movements for 15 containers
            var ports = await _context.Ports.ToListAsync();
            var users = await _context.Users.ToListAsync();

            if (!containers.Any() || !ports.Any() || !users.Any())
            {
                _logger.LogWarning("Required data not found for container movements seeding");
                return;
            }

            var movements = new List<ContainerMovement>();
            var movementTypes = new[] { "Loading", "Unloading", "Transfer", "Storage", "Inspection", "Customs" };

            foreach (var container in containers)
            {
                // Create 1-3 movement records per container (lean approach)
                var movementCount = _random.Next(1, 4);
                
                for (int i = 0; i < movementCount; i++)
                {
                    var movement = new ContainerMovement
                    {
                        ContainerId = container.ContainerId,
                        MovementType = movementTypes[i % movementTypes.Length],
                        FromLocation = i == 0 ? "Origin Terminal" : $"Stage {i}",
                        ToLocation = i == movementCount - 1 ? container.CurrentLocation : $"Stage {i + 1}",
                        MovementTimestamp = container.CreatedAt.AddHours(i * _random.Next(6, 24)),
                        Status = i == movementCount - 1 ? "In Progress" : "Completed",
                        Coordinates = GenerateRealisticCoordinates(),
                        Notes = $"Movement {i + 1}: {movementTypes[i % movementTypes.Length]} operation",
                        PortId = ports[_random.Next(ports.Count)].PortId,
                        RecordedByUserId = users[_random.Next(users.Count)].UserId,
                        CreatedAt = container.CreatedAt.AddHours(i * 12),
                        EstimatedCompletion = container.CreatedAt.AddHours(i * 12 + _random.Next(2, 8)),
                        ActualCompletion = i == movementCount - 1 ? null : container.CreatedAt.AddHours(i * 12 + _random.Next(2, 6))
                    };

                    // Add environmental data for refrigerated containers
                    if (container.Type == "Refrigerated")
                    {
                        movement.Temperature = _random.Next(-20, 10);
                        movement.Humidity = _random.Next(40, 80);
                    }

                    movements.Add(movement);
                }
            }

            _context.ContainerMovements.AddRange(movements);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} lean container movements", movements.Count);
        }

        private async Task SeedEventsAsync()
        {
            _logger.LogInformation("Seeding lean events...");

            var containers = await _context.Containers.Take(10).ToListAsync();
            var ships = await _context.Ships.ToListAsync();
            var berths = await _context.Berths.Take(10).ToListAsync();
            var ports = await _context.Ports.ToListAsync();
            var users = await _context.Users.ToListAsync();

            var events = new List<Event>();
            var eventTypes = new[] 
            { 
                "Ship Arrival", "Ship Departure", "Container Loading", "Container Unloading",
                "Berth Assignment", "Equipment Failure", "Security Alert", "Weather Warning",
                "Customs Inspection", "Maintenance Scheduled", "Delay Notification", "Cargo Damage"
            };
            
            var priorities = new[] { "Low", "Medium", "High", "Critical" };

            // Create exactly 20 events covering all types
            for (int i = 0; i < 20; i++)
            {
                var eventType = eventTypes[i % eventTypes.Length]; // Cycle through all types
                var priority = priorities[i % priorities.Length]; // Cycle through all priorities
                
                var eventObj = new Event
                {
                    EventType = eventType,
                    Title = GenerateEventTitle(eventType),
                    Description = GenerateEventDescription(eventType),
                    Priority = priority,
                    Status = i % 4 == 0 ? "Resolved" : i % 3 == 0 ? "Acknowledged" : "New",
                    RequiresAction = priority == "High" || priority == "Critical",
                    EventTimestamp = DateTime.UtcNow.AddHours(-_random.Next(1, 168)),
                    CreatedAt = DateTime.UtcNow.AddHours(-_random.Next(1, 168))
                };

                // Assign to entities systematically
                switch (eventType)
                {
                    case "Container Loading":
                    case "Container Unloading":
                    case "Cargo Damage":
                        if (containers.Any())
                            eventObj.ContainerId = containers[i % containers.Count].ContainerId;
                        break;
                        
                    case "Ship Arrival":
                    case "Ship Departure":
                        if (ships.Any())
                            eventObj.ShipId = ships[i % ships.Count].ShipId;
                        break;
                        
                    case "Berth Assignment":
                    case "Equipment Failure":
                        if (berths.Any())
                            eventObj.BerthId = berths[i % berths.Count].BerthId;
                        break;
                        
                    default:
                        if (ports.Any())
                            eventObj.PortId = ports[i % ports.Count].PortId;
                        break;
                }

                // Assign to user if requires action
                if (eventObj.RequiresAction && users.Any())
                {
                    eventObj.AssignedToUserId = users[i % users.Count].UserId;
                }

                events.Add(eventObj);
            }

            _context.Events.AddRange(events);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} lean events covering all types", events.Count);
        }

        private async Task SeedAnalyticsDataAsync()
        {
            _logger.LogInformation("Seeding analytics data...");

            var ports = await _context.Ports.ToListAsync();
            var berths = await _context.Berths.ToListAsync();

            var analytics = new List<Analytics>();
            var metricTypes = new[] 
            { 
                "Throughput", "Utilization", "Turnaround Time", "Efficiency", 
                "Revenue", "Costs", "Delays", "Performance Score" 
            };

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
                            Value = GenerateRealisticMetricValue(metricType),
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
                    var date = DateTime.UtcNow.AddDays(-7 * week);
                    
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

