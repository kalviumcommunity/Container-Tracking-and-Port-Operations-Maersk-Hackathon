using Backend.Constants;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Backend.Data.Seeding
{
    /// <summary>
    /// Clean, production-ready data seeder for Azure PostgreSQL deployment
    /// </summary>
    public static class ProductionDataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Starting production data seeding...");

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Seed in correct order due to foreign key dependencies
                await SeedPermissions(context, logger);
                await SeedRoles(context, logger);
                await SeedRolePermissions(context, logger);
                await SeedUsers(context, logger);
                await SeedPorts(context, logger);
                await SeedBerths(context, logger);
                await SeedShips(context, logger);
                await SeedContainers(context, logger);
                await SeedBerthAssignments(context, logger);

                logger.LogInformation("Production data seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during production data seeding");
                throw;
            }
        }

        private static async Task SeedRoles(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Roles.AnyAsync())
            {
                logger.LogInformation("Roles already exist, skipping role seeding");
                return;
            }

            var roles = new List<Role>
            {
                new() 
                { 
                    Name = Roles.Admin, 
                    Description = "Full system administration access",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new() 
                { 
                    Name = Roles.PortManager, 
                    Description = "Manage port operations and assign roles",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new() 
                { 
                    Name = Roles.Operator, 
                    Description = "Handle container operations and movements",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                },
                new() 
                { 
                    Name = Roles.Viewer, 
                    Description = "Read-only access to system information",
                    IsSystemRole = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {roles.Count} roles");
        }

        private static async Task SeedUsers(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist, skipping user seeding");
                return;
            }

            // Use the same password hashing method as AuthService
            var defaultPassword = "SecurePass123!";

            var users = new List<User>
            {
                new()
                {
                    Username = "admin",
                    Email = "admin@containertrack.com",
                    FullName = "System Administrator",
                    Department = "IT",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = HashPassword(defaultPassword)
                },
                new()
                {
                    Username = "portmanager",
                    Email = "manager@containertrack.com", 
                    FullName = "Port Manager",
                    Department = "Operations",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = HashPassword(defaultPassword)
                },
                new()
                {
                    Username = "operator1",
                    Email = "operator1@containertrack.com",
                    FullName = "Container Operator",
                    Department = "Yard Operations",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = HashPassword(defaultPassword)
                },
                new()
                {
                    Username = "viewer1",
                    Email = "viewer@containertrack.com",
                    FullName = "Operations Viewer",
                    Department = "Logistics",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = HashPassword(defaultPassword)
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Assign roles to users
            await AssignUserRoles(context, logger);
            
            logger.LogInformation($"Seeded {users.Count} users with secure passwords");
        }

        private static async Task AssignUserRoles(ApplicationDbContext context, ILogger logger)
        {
            var adminUser = await context.Users.FirstAsync(u => u.Username == "admin");
            var managerUser = await context.Users.FirstAsync(u => u.Username == "portmanager");
            var operatorUser = await context.Users.FirstAsync(u => u.Username == "operator1");
            var viewerUser = await context.Users.FirstAsync(u => u.Username == "viewer1");

            var adminRole = await context.Roles.FirstAsync(r => r.Name == Roles.Admin);
            var managerRole = await context.Roles.FirstAsync(r => r.Name == Roles.PortManager);
            var operatorRole = await context.Roles.FirstAsync(r => r.Name == Roles.Operator);
            var viewerRole = await context.Roles.FirstAsync(r => r.Name == Roles.Viewer);

            var userRoles = new List<UserRole>
            {
                new() 
                { 
                    UserId = adminUser.UserId, 
                    RoleId = adminRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                },
                new() 
                { 
                    UserId = managerUser.UserId, 
                    RoleId = managerRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                },
                new() 
                { 
                    UserId = operatorUser.UserId, 
                    RoleId = operatorRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                },
                new() 
                { 
                    UserId = viewerUser.UserId, 
                    RoleId = viewerRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                }
            };

            await context.UserRoles.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
            logger.LogInformation("User roles assigned successfully");
        }

        private static async Task SeedPorts(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Ports.AnyAsync())
            {
                logger.LogInformation("Ports already exist, skipping port seeding");
                return;
            }

            var ports = new List<Port>
            {
                new()
                {
                    Name = "Port of Hamburg",
                    Code = "DEHAM", 
                    Location = "Hamburg, Germany",
                    Country = "Germany",
                    Coordinates = "53.5488,9.9872",
                    TotalContainerCapacity = 150000,
                    MaxShipCapacity = 50,
                    Status = "Active",
                    TimeZone = "Europe/Berlin",
                    OperatingHours = "24/7",
                    Services = "Container Terminal, Ro-Ro, Bulk Cargo",
                    ContactInfo = "info@hamburg-port.de",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Port of Rotterdam",
                    Code = "NLRTM",
                    Location = "Rotterdam, Netherlands",
                    Country = "Netherlands",
                    Coordinates = "51.9244,4.4777",
                    TotalContainerCapacity = 200000,
                    MaxShipCapacity = 75,
                    Status = "Active",
                    TimeZone = "Europe/Amsterdam",
                    OperatingHours = "24/7",
                    Services = "Container Terminal, Oil Refinery, Chemical Hub",
                    ContactInfo = "info@portofrotterdam.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Port of Antwerp",
                    Code = "BEANR",
                    Location = "Antwerp, Belgium",
                    Country = "Belgium",
                    Coordinates = "51.2194,4.4025",
                    TotalContainerCapacity = 120000,
                    MaxShipCapacity = 40,
                    Status = "Active",
                    TimeZone = "Europe/Brussels",
                    OperatingHours = "24/7",
                    Services = "Container Terminal, Break Bulk, Automotive",
                    ContactInfo = "info@portofantwerp.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Ports.AddRangeAsync(ports);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {ports.Count} ports");
        }

        private static async Task SeedBerths(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Berths.AnyAsync())
            {
                logger.LogInformation("Berths already exist, skipping berth seeding");
                return;
            }

            var ports = await context.Ports.ToListAsync();
            var berths = new List<Berth>();

            foreach (var port in ports)
            {
                for (int i = 1; i <= 5; i++)
                {
                    berths.Add(new Berth
                    {
                        Name = $"Berth {i}",
                        Identifier = $"{port.Code}-B{i:D2}",
                        PortId = port.PortId,
                        Status = i <= 3 ? "Available" : "Occupied",
                        Type = i % 2 == 0 ? "Container" : "General Cargo",
                        MaxShipLength = 400,
                        MaxDraft = 15.0m,
                        Capacity = 5000,
                        CurrentLoad = 0,
                        CraneCount = i <= 2 ? 4 : 2,
                        HourlyRate = 500.00m,
                        AvailableServices = "Loading, Unloading, Storage",
                        Priority = i <= 2 ? "High" : "Standard",
                        Notes = $"Standard berth facility for {port.Name}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await context.Berths.AddRangeAsync(berths);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {berths.Count} berths");
        }

        private static async Task SeedShips(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Ships.AnyAsync())
            {
                logger.LogInformation("Ships already exist, skipping ship seeding");
                return;
            }

            var ports = await context.Ports.ToListAsync();
            var random = new Random(42); // Fixed seed for consistent data

            var ships = new List<Ship>
            {
                new()
                {
                    Name = "Atlantic Pioneer",
                    ImoNumber = "IMO9876543",
                    Flag = "Denmark",
                    Type = "Container Ship",
                    Length = 350,
                    Beam = 45,
                    Draft = 14.5m,
                    GrossTonnage = 120000,
                    Capacity = 18000,
                    Status = "In Transit",
                    Speed = 22.5m,
                    Heading = 045.0m,
                    Coordinates = "55.7558,12.6015",
                    CurrentPortId = null,
                    NextPort = "Hamburg",
                    EstimatedArrival = DateTime.UtcNow.AddHours(18),
                    YearBuilt = 2018,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Nordic Explorer", 
                    ImoNumber = "IMO9876544",
                    Flag = "Norway",
                    Type = "Container Ship",
                    Length = 320,
                    Beam = 42,
                    Draft = 13.5m,
                    GrossTonnage = 95000,
                    Capacity = 15000,
                    Status = "At Port",
                    Speed = 0,
                    Heading = 0,
                    Coordinates = "51.9244,4.4777",
                    CurrentPortId = ports.First(p => p.Code == "NLRTM").PortId,
                    NextPort = "Antwerp",
                    EstimatedArrival = DateTime.UtcNow.AddHours(24),
                    YearBuilt = 2020,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Name = "Baltic Carrier",
                    ImoNumber = "IMO9876545",
                    Flag = "Germany",
                    Type = "Container Ship",
                    Length = 280,
                    Beam = 38,
                    Draft = 12.0m,
                    GrossTonnage = 75000,
                    Capacity = 12000,
                    Status = "Loading",
                    Speed = 0,
                    Heading = 0,
                    Coordinates = "53.5488,9.9872",
                    CurrentPortId = ports.First(p => p.Code == "DEHAM").PortId,
                    NextPort = "Rotterdam",
                    EstimatedArrival = DateTime.UtcNow.AddHours(36),
                    YearBuilt = 2019,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Ships.AddRangeAsync(ships);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {ships.Count} ships");
        }

        private static async Task SeedContainers(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Containers.AnyAsync())
            {
                logger.LogInformation("Containers already exist, skipping container seeding");
                return;
            }

            var ships = await context.Ships.ToListAsync();
            var containers = new List<Container>();
            var containerTypes = new[] { "Standard", "High Cube", "Refrigerated", "Open Top", "Flat Rack" };
            var statuses = new[] { "Loading", "In Transit", "Discharged", "Available", "Empty" };
            var cargoTypes = new[] { "General Cargo", "Electronics", "Automotive Parts", "Textiles", "Food Products", "Chemicals", "Machinery" };
            var destinations = new[] { "Hamburg", "Rotterdam", "Antwerp", "Bremen", "Amsterdam", "Ghent" };

            var random = new Random(42); // Fixed seed for consistent data

            for (int i = 1; i <= 100; i++)
            {
                var size = random.Next(2) == 0 ? 20 : 40;
                var maxWeight = size == 20 ? 28000 : 32500;
                var currentWeight = random.Next(5000, maxWeight);
                var containerType = containerTypes[random.Next(containerTypes.Length)];

                containers.Add(new Container
                {
                    ContainerId = $"DEMO{i:D6}",
                    Type = containerType,
                    Size = size == 20 ? "20ft" : "40ft",
                    Weight = currentWeight,
                    MaxWeight = maxWeight,
                    Status = statuses[random.Next(statuses.Length)],
                    CargoType = cargoTypes[random.Next(cargoTypes.Length)],
                    CargoDescription = $"Commercial goods - Lot {i}",
                    Destination = destinations[random.Next(destinations.Length)],
                    CurrentLocation = ships[random.Next(ships.Count)].Name,
                    ShipId = ships[random.Next(ships.Count)].ShipId,
                    Coordinates = $"{random.Next(50, 56)}.{random.Next(1000, 9999)},{random.Next(3, 13)}.{random.Next(1000, 9999)}",
                    Temperature = containerType == "Refrigerated" ? random.Next(-20, 5) : null,
                    Condition = random.Next(10) > 7 ? "Damaged" : "Good",
                    EstimatedArrival = DateTime.UtcNow.AddHours(random.Next(6, 168)), // 6 hours to 1 week
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.Containers.AddRangeAsync(containers);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {containers.Count} containers");
        }

        private static async Task SeedBerthAssignments(ApplicationDbContext context, ILogger logger)
        {
            if (await context.BerthAssignments.AnyAsync())
            {
                logger.LogInformation("Berth assignments already exist, skipping assignment seeding");
                return;
            }

            var shipsAtPort = await context.Ships
                .Where(s => s.Status == "At Port" || s.Status == "Loading")
                .ToListAsync();
            
            var availableBerths = await context.Berths
                .Where(b => b.Status == "Available")
                .Include(b => b.Port)
                .Take(shipsAtPort.Count)
                .ToListAsync();

            var assignments = new List<BerthAssignment>();
            var random = new Random(42);

            for (int i = 0; i < Math.Min(shipsAtPort.Count, availableBerths.Count); i++)
            {
                var assignedAt = DateTime.UtcNow.AddHours(-random.Next(1, 48));
                var estimatedDeparture = assignedAt.AddHours(random.Next(12, 72));

                assignments.Add(new BerthAssignment
                {
                    ShipId = shipsAtPort[i].ShipId,
                    BerthId = availableBerths[i].BerthId,
                    AssignedAt = assignedAt,
                    ScheduledDeparture = estimatedDeparture,
                    Status = "Active",
                    AssignmentType = "Loading",
                    Priority = "Medium",
                    Notes = $"Standard loading/unloading operation for {shipsAtPort[i].Name} at {availableBerths[i].Port?.Name}",
                    CreatedAt = assignedAt,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.BerthAssignments.AddRangeAsync(assignments);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {assignments.Count} berth assignments");
        }

        /// <summary>
        /// Hash password using ASP.NET Core Identity's secure PasswordHasher with salt
        /// </summary>
        private static string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(new User(), password);
        }

        /// <summary>
        /// Seed system permissions
        /// </summary>
        private static async Task SeedPermissions(ApplicationDbContext context, ILogger logger)
        {
            var existingPermissions = await context.Permissions.Select(p => p.Name).ToListAsync();

            var permissions = new List<Permission>
            {
                // Global permissions
                new() { Name = Permissions.GlobalPortAccess, Description = "Access to all ports in the system", Category = "Global", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ManageAllPorts, Description = "Manage all ports in the system", Category = "Global", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ManageUsers, Description = "Create, update, and delete users", Category = "User Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ManageRoles, Description = "Create, update, and delete roles", Category = "User Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewSystemReports, Description = "View system-wide reports and analytics", Category = "Reports", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },

                // Port management permissions
                new() { Name = Permissions.ManagePortDetails, Description = "Update port information and settings", Category = "Port Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewPortDetails, Description = "View port information and details", Category = "Port Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewPortReports, Description = "View port-specific reports", Category = "Reports", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },

                // Container permissions
                new() { Name = Permissions.ManageContainers, Description = "Create, update, and delete container records", Category = "Container Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewContainers, Description = "View container information", Category = "Container Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.TrackContainers, Description = "Track container movements and status", Category = "Container Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },

                // Ship permissions
                new() { Name = Permissions.ManageShips, Description = "Create, update, and delete ship records", Category = "Ship Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewShips, Description = "View ship information", Category = "Ship Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ScheduleShips, Description = "Schedule ship arrivals and departures", Category = "Ship Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },

                // Cargo permissions
                new() { Name = Permissions.ManageCargo, Description = "Create, update, and delete cargo records", Category = "Cargo Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewCargo, Description = "View cargo information", Category = "Cargo Operations", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },

                // Berth permissions
                new() { Name = Permissions.ManageBerths, Description = "Create, update, and delete berth records", Category = "Berth Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewBerths, Description = "View berth information", Category = "Berth Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.AllocateBerths, Description = "Allocate berths to ships", Category = "Berth Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewBerthAssignments, Description = "View berth assignment information", Category = "Berth Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },

                // Equipment permissions
                new() { Name = Permissions.ManageEquipment, Description = "Create, update, and delete equipment records", Category = "Equipment Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow },
                new() { Name = Permissions.ViewEquipment, Description = "View equipment information", Category = "Equipment Management", IsSystemPermission = true, CreatedAt = DateTime.UtcNow }
            };

            foreach (var permission in permissions)
            {
                if (!existingPermissions.Contains(permission.Name))
                {
                    await context.Permissions.AddAsync(permission);
                    logger.LogInformation($"Added permission: {permission.Name}");
                }
            }

            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded permissions - added {permissions.Count(p => !existingPermissions.Contains(p.Name))} new permissions");
        }

        /// <summary>
        /// Seed role-permission relationships
        /// </summary>
        private static async Task SeedRolePermissions(ApplicationDbContext context, ILogger logger)
        {
            var existingRolePermissions = await context.RolePermissions.ToListAsync();

            var roles = await context.Roles.ToListAsync();
            var permissions = await context.Permissions.ToListAsync();

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
                    Permissions.ViewBerthAssignments,
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
                    Permissions.ViewBerths, Permissions.ViewBerthAssignments,
                    Permissions.ViewEquipment
                }
            };

            var rolePermissionsToAdd = new List<RolePermission>();

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
                        rolePermissionsToAdd.Add(new RolePermission
                        {
                            RoleId = role.RoleId,
                            PermissionId = permission.PermissionId,
                            GrantedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await context.RolePermissions.AddRangeAsync(rolePermissionsToAdd);
            await context.SaveChangesAsync();
            logger.LogInformation($"Seeded {rolePermissionsToAdd.Count} role-permission relationships");
        }
    }
}