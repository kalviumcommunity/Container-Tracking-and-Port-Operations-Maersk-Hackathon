using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Backend.Data.Seeding
{
    /// <summary>
    /// Seeds comprehensive sample data into the enhanced database schema for testing purposes
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds the database with comprehensive sample data for the enhanced schema
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="forceReseed">Force clear and reseed all data</param>
        public static async Task SeedAsync(ApplicationDbContext context, bool forceReseed = false)
        {
            // Connect to ContainerTrackingDB with enhanced schema
            
            if (forceReseed)
            {
                // Force reseeding enabled - clearing existing enhanced data
                // Clear only the enhanced data tables, not authentication data
                context.Analytics.RemoveRange(await context.Analytics.ToListAsync());
                context.ContainerMovements.RemoveRange(await context.ContainerMovements.ToListAsync());
                context.Events.RemoveRange(await context.Events.ToListAsync());
                context.BerthAssignments.RemoveRange(await context.BerthAssignments.ToListAsync());
                context.Containers.RemoveRange(await context.Containers.ToListAsync());
                context.Ships.RemoveRange(await context.Ships.ToListAsync());
                context.Berths.RemoveRange(await context.Berths.ToListAsync());
                context.Ports.RemoveRange(await context.Ports.ToListAsync());
                await context.SaveChangesAsync();
                // Existing enhanced data cleared successfully
            }
            
            // Seed in proper order to maintain foreign key relationships
            await SeedUsers(context);
            await SeedPorts(context);
            await SeedBerths(context);
            await SeedShips(context);
            await SeedContainers(context);
            await SeedBerthAssignments(context);
            await SeedEvents(context);
            await SeedContainerMovements(context);
            await SeedAnalytics(context);

            // Enhanced schema data seeding completed successfully
        }

        private static async Task SeedUsers(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                Console.WriteLine("Users already exist, skipping user seeding...");
                return;
            }

            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    FullName = "System Administrator",
                    Email = "admin@maersk.com",
                    Department = "IT Administration",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "operator1",
                    FullName = "John Hansen",
                    Email = "john.hansen@maersk.com",
                    Department = "Port Operations",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "supervisor1",
                    FullName = "Sarah Nielsen",
                    Email = "sarah.nielsen@maersk.com",
                    Department = "Port Management",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Username = "controller1",
                    FullName = "Lars Andersen",
                    Email = "lars.andersen@maersk.com",
                    Department = "Traffic Control",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {users.Count} users.");
        }

        private static async Task SeedPorts(ApplicationDbContext context)
        {
            if (await context.Ports.AnyAsync())
            {
                Console.WriteLine("Ports already exist, skipping port seeding...");
                return;
            }

            var ports = new List<Port>
            {
                new Port
                {
                    Name = "APM Terminals Copenhagen",
                    Code = "DKCPH",
                    Location = "Copenhagen",
                    Country = "Denmark",
                    Coordinates = "55.6761,12.5683",
                    TotalContainerCapacity = 10000,
                    MaxShipCapacity = 25,
                    CurrentShipCount = 8,
                    CurrentContainerCount = 3247,
                    Status = "Operational",
                    ContactInfo = "Phone: +45 3374 3374, Email: cph@apmterminals.com",
                    Services = "Container handling, Warehousing, Customs clearance, Ship chandling",
                    OperatingHours = "24/7",
                    TimeZone = "CET",
                    CreatedAt = DateTime.UtcNow.AddDays(-180),
                    UpdatedAt = DateTime.UtcNow
                },
                new Port
                {
                    Name = "Port of Rotterdam",
                    Code = "NLRTM",
                    Location = "Rotterdam",
                    Country = "Netherlands",
                    Coordinates = "51.9244,4.4777",
                    TotalContainerCapacity = 15000,
                    MaxShipCapacity = 40,
                    CurrentShipCount = 15,
                    CurrentContainerCount = 8934,
                    Status = "Operational",
                    ContactInfo = "Phone: +31 10 252 1010, Email: info@portofrotterdam.com",
                    Services = "Container handling, Bulk cargo, Oil refining, Distribution",
                    OperatingHours = "24/7",
                    TimeZone = "CET",
                    CreatedAt = DateTime.UtcNow.AddDays(-200),
                    UpdatedAt = DateTime.UtcNow
                },
                new Port
                {
                    Name = "Port of Hamburg",
                    Code = "DEHAM",
                    Location = "Hamburg",
                    Country = "Germany",
                    Coordinates = "53.5511,9.9937",
                    TotalContainerCapacity = 12000,
                    MaxShipCapacity = 30,
                    CurrentShipCount = 12,
                    CurrentContainerCount = 6521,
                    Status = "Operational",
                    ContactInfo = "Phone: +49 40 42847 0, Email: info@hafen-hamburg.de",
                    Services = "Container terminal, Rail connection, Inland shipping",
                    OperatingHours = "24/7",
                    TimeZone = "CET",
                    CreatedAt = DateTime.UtcNow.AddDays(-190),
                    UpdatedAt = DateTime.UtcNow
                },
                new Port
                {
                    Name = "Port of Antwerp",
                    Code = "BEANR",
                    Location = "Antwerp",
                    Country = "Belgium",
                    Coordinates = "51.2194,4.4025",
                    TotalContainerCapacity = 8000,
                    MaxShipCapacity = 22,
                    CurrentShipCount = 9,
                    CurrentContainerCount = 4123,
                    Status = "Operational",
                    ContactInfo = "Phone: +32 3 205 20 11, Email: info@portofantwerp.com",
                    Services = "Container handling, Chemical cluster, Automotive",
                    OperatingHours = "24/7",
                    TimeZone = "CET",
                    CreatedAt = DateTime.UtcNow.AddDays(-170),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Ports.AddRangeAsync(ports);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {ports.Count} ports.");
        }

        private static async Task SeedBerths(ApplicationDbContext context)
        {
            if (await context.Berths.AnyAsync())
            {
                Console.WriteLine("Berths already exist, skipping berth seeding...");
                return;
            }

            var ports = await context.Ports.ToListAsync();
            if (!ports.Any())
            {
                Console.WriteLine("No ports found, cannot seed berths.");
                return;
            }

            var berths = new List<Berth>();
            var berthTypes = new[] { "Container", "Bulk", "RoRo", "General Cargo", "Oil", "LNG" };

            foreach (var port in ports)
            {
                var berthCount = port.Code == "NLRTM" ? 8 : port.Code == "DEHAM" ? 6 : 5;
                
                for (int i = 1; i <= berthCount; i++)
                {
                    var random = new Random(port.PortId * 100 + i);
                    berths.Add(new Berth
                    {
                        PortId = port.PortId,
                        Name = $"Berth {i}",
                        Identifier = $"{port.Code}-B{i:D2}",
                        Type = berthTypes[i % berthTypes.Length],
                        Status = i % 4 == 0 ? "Maintenance" : (i % 3 == 0 ? "Occupied" : "Available"),
                        Capacity = 500 + random.Next(300, 800),
                        CurrentLoad = i % 3 == 0 ? random.Next(100, 400) : 0,
                        MaxShipLength = 300 + random.Next(50, 100),
                        MaxDraft = 12.0m + random.Next(1, 6),
                        CraneCount = random.Next(2, 6),
                        AvailableServices = "Loading, Unloading, Storage, Inspection",
                        HourlyRate = 500 + random.Next(100, 300),
                        Priority = i <= 2 ? "High" : (i <= 4 ? "Medium" : "Low"),
                        Notes = $"Modern container berth with automated equipment",
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 180)),
                        UpdatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 24))
                    });
                }
            }

            await context.Berths.AddRangeAsync(berths);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {berths.Count} berths across {ports.Count} ports.");
        }

        private static async Task SeedShips(ApplicationDbContext context)
        {
            if (await context.Ships.AnyAsync())
            {
                Console.WriteLine("Ships already exist, skipping ship seeding...");
                return;
            }

            var ports = await context.Ports.ToListAsync();
            var ships = new List<Ship>
            {
                new Ship
                {
                    Name = "Maersk Edinburgh",
                    ImoNumber = "IMO9778474",
                    Type = "Container Ship",
                    Status = "Docked",
                    Flag = "Denmark",
                    Capacity = 14000,
                    Length = 399.0m,
                    Beam = 59.0m,
                    Draft = 16.0m,
                    GrossTonnage = 165000,
                    YearBuilt = 2018,
                    CurrentPortId = ports.FirstOrDefault(p => p.Code == "DKCPH")?.PortId,
                    Coordinates = "55.6761,12.5683",
                    Speed = 0,
                    Heading = 45,
                    NextPort = "Rotterdam",
                    EstimatedArrival = DateTime.UtcNow.AddHours(18),
                    CreatedAt = DateTime.UtcNow.AddDays(-120),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-15)
                },
                new Ship
                {
                    Name = "MSC Gulsun",
                    ImoNumber = "IMO9811000",
                    Type = "Container Ship",
                    Status = "At Sea",
                    Flag = "Panama",
                    Capacity = 23000,
                    Length = 400.0m,
                    Beam = 61.0m,
                    Draft = 16.5m,
                    GrossTonnage = 232618,
                    YearBuilt = 2019,
                    Coordinates = "52.3456,3.8794",
                    Speed = 18.5m,
                    Heading = 285,
                    NextPort = "Rotterdam",
                    EstimatedArrival = DateTime.UtcNow.AddHours(6),
                    CreatedAt = DateTime.UtcNow.AddDays(-95),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
                },
                new Ship
                {
                    Name = "COSCO Shipping Universe",
                    ImoNumber = "IMO9795815",
                    Type = "Container Ship",
                    Status = "Loading",
                    Flag = "China",
                    Capacity = 21000,
                    Length = 400.0m,
                    Beam = 58.6m,
                    Draft = 16.0m,
                    GrossTonnage = 199629,
                    YearBuilt = 2018,
                    CurrentPortId = ports.FirstOrDefault(p => p.Code == "DEHAM")?.PortId,
                    Coordinates = "53.5511,9.9937",
                    Speed = 0,
                    Heading = 180,
                    NextPort = "Antwerp",
                    EstimatedArrival = DateTime.UtcNow.AddHours(24),
                    CreatedAt = DateTime.UtcNow.AddDays(-87),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-8)
                }
            };

            await context.Ships.AddRangeAsync(ships);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {ships.Count} ships.");
        }

        private static async Task SeedContainers(ApplicationDbContext context)
        {
            var existingContainers = await context.Containers.ToListAsync();
            if (existingContainers.Any())
            {
                Console.WriteLine("Containers already exist, updating with enhanced data...");
                
                var updateSizes = new[] { "20ft", "40ft", "45ft" };
                var updateConditions = new[] { "Good", "Excellent", "Fair", "Damaged" };
                var updateDestinations = new[] { "Shanghai", "Singapore", "Los Angeles", "Hamburg", "Rotterdam", "Dubai" };
                var updateCargoDescriptions = new[] { 
                    "Electronics", "Automotive Parts", "Textiles", "Machinery", "Food Products", 
                    "Chemicals", "Pharmaceuticals", "Consumer Goods", "Raw Materials", "Furniture" 
                };

                var updateShips = await context.Ships.ToListAsync();
                var updateRandom = new Random(42); // Fixed seed for consistent results

                for (int i = 0; i < existingContainers.Count; i++)
                {
                    var container = existingContainers[i];
                    var hasShip = updateRandom.Next(0, 3) > 0 && updateShips.Any();
                    var ship = hasShip ? updateShips[updateRandom.Next(updateShips.Count)] : null;
                    var size = updateSizes[updateRandom.Next(updateSizes.Length)];
                    var maxWeight = size == "20ft" ? 25000 : size == "40ft" ? 30000 : 32000;

                    // Update with enhanced data
                    container.CargoDescription = updateCargoDescriptions[updateRandom.Next(updateCargoDescriptions.Length)];
                    container.Condition = updateConditions[updateRandom.Next(updateConditions.Length)];
                    container.Temperature = updateRandom.Next(0, 10) > 7 ? updateRandom.Next(-18, 2) : null;
                    container.Destination = updateDestinations[updateRandom.Next(updateDestinations.Length)];
                    container.CurrentLocation = ship?.CurrentPort?.Name ?? "Container Yard";
                    container.Coordinates = ship?.Coordinates ?? "55.6761,12.5683";
                    container.EstimatedArrival = DateTime.UtcNow.AddDays(updateRandom.Next(1, 14));
                    container.Size = size;
                    container.Weight = updateRandom.Next(5000, (int)(maxWeight * 0.8m));
                    container.MaxWeight = maxWeight;
                    container.Type = updateRandom.Next(0, 10) > 7 ? "Refrigerated" : "Dry";
                    container.Status = ship == null ? "In Port" : ship.Status == "Docked" ? "Loading" : "In Transit";
                    container.UpdatedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
                Console.WriteLine($"Updated {existingContainers.Count} containers with enhanced data!");
                return;
            }

            var ships = await context.Ships.ToListAsync();
            var containers = new List<Container>();

            var containerSizes = new[] { "20ft", "40ft", "45ft" };
            var conditions = new[] { "Good", "Excellent", "Fair", "Damaged" };
            var destinations = new[] { "Shanghai", "Singapore", "Los Angeles", "Hamburg", "Rotterdam", "Dubai" };
            var cargoDescriptions = new[] { 
                "Electronics", "Automotive Parts", "Textiles", "Machinery", "Food Products", 
                "Chemicals", "Pharmaceuticals", "Consumer Goods", "Raw Materials", "Furniture" 
            };

            for (int i = 1; i <= 50; i++)
            {
                var random = new Random(i);
                var hasShip = random.Next(0, 3) > 0; // 66% chance to be on a ship
                var ship = hasShip && ships.Any() ? ships[random.Next(ships.Count)] : null;
                var size = containerSizes[random.Next(containerSizes.Length)];
                var maxWeight = size == "20ft" ? 25000 : size == "40ft" ? 30000 : 32000;
                
                containers.Add(new Container
                {
                    ContainerId = GenerateContainerId(i),
                    ContainerNumber = GenerateContainerNumber(i),
                    Name = $"Container {i:D3}",
                    Type = random.Next(0, 10) > 7 ? "Refrigerated" : "Dry",
                    Status = ship == null ? "In Port" : ship.Status == "Docked" ? "Loading" : "In Transit",
                    Size = size,
                    Weight = random.Next(5000, (int)(maxWeight * 0.8m)),
                    MaxWeight = maxWeight,
                    Condition = conditions[random.Next(conditions.Length)],
                    Temperature = random.Next(0, 10) > 7 ? random.Next(-18, 2) : null,
                    Destination = destinations[random.Next(destinations.Length)],
                    CargoDescription = cargoDescriptions[random.Next(cargoDescriptions.Length)],
                    CurrentLocation = ship?.CurrentPort?.Name ?? "Container Yard",
                    Coordinates = ship?.Coordinates ?? "55.6761,12.5683",
                    EstimatedArrival = DateTime.UtcNow.AddDays(random.Next(1, 14)),
                    ShipId = ship?.ShipId,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                    UpdatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 24))
                });
            }

            await context.Containers.AddRangeAsync(containers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {containers.Count} containers.");
        }

        private static async Task SeedBerthAssignments(ApplicationDbContext context)
        {
            if (await context.BerthAssignments.AnyAsync())
            {
                Console.WriteLine("Berth assignments already exist, skipping seeding...");
                return;
            }

            var berths = await context.Berths.ToListAsync();
            var ships = await context.Ships.ToListAsync();
            var containers = await context.Containers.ToListAsync();
            var users = await context.Users.ToListAsync();
            
            if (!berths.Any() || !ships.Any() || !users.Any())
            {
                Console.WriteLine("Missing required data for berth assignments.");
                return;
            }

            var assignments = new List<BerthAssignment>();
            var assignmentTypes = new[] { "Loading", "Unloading", "Maintenance", "Inspection" };
            var priorities = new[] { "High", "Medium", "Low" };
            var statuses = new[] { "Scheduled", "Active", "Completed", "Cancelled" };

            for (int i = 0; i < 25; i++)
            {
                var random = new Random(i);
                var berth = berths[random.Next(berths.Count)];
                var ship = random.Next(0, 3) > 0 ? ships[random.Next(ships.Count)] : null;
                var container = random.Next(0, 4) > 0 ? containers[random.Next(containers.Count)] : null;
                var user = users[random.Next(users.Count)];
                var type = assignmentTypes[random.Next(assignmentTypes.Length)];
                var status = statuses[random.Next(statuses.Length)];
                var scheduled = DateTime.UtcNow.AddDays(-random.Next(0, 10)).AddHours(random.Next(0, 24));
                
                assignments.Add(new BerthAssignment
                {
                    BerthId = berth.BerthId,
                    ShipId = ship?.ShipId,
                    ContainerId = container?.ContainerId,
                    AssignmentType = type,
                    Status = status,
                    Priority = priorities[random.Next(priorities.Length)],
                    ScheduledArrival = scheduled,
                    ScheduledDeparture = scheduled.AddHours(random.Next(4, 24)),
                    ActualArrival = status != "Scheduled" ? scheduled.AddMinutes(random.Next(-30, 60)) : null,
                    ActualDeparture = status == "Completed" ? scheduled.AddHours(random.Next(4, 20)) : null,
                    EstimatedProcessingTime = random.Next(240, 1440), // 4-24 hours in minutes
                    ContainerCount = random.Next(50, 300),
                    Cost = random.Next(5000, 25000),
                    CreatedByUserId = user.UserId,
                    Notes = $"Assignment for {type.ToLower()} operations",
                    AssignedAt = scheduled.AddDays(-1),
                    CreatedAt = scheduled.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-random.Next(1, 24))
                });
            }

            await context.BerthAssignments.AddRangeAsync(assignments);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {assignments.Count} berth assignments.");
        }

        private static async Task SeedEvents(ApplicationDbContext context)
        {
            if (await context.Events.AnyAsync())
            {
                Console.WriteLine("Events already exist, skipping event seeding...");
                return;
            }

            var containers = await context.Containers.ToListAsync();
            var ships = await context.Ships.ToListAsync();
            var berths = await context.Berths.ToListAsync();
            var ports = await context.Ports.ToListAsync();
            var users = await context.Users.ToListAsync();

            if (!containers.Any() || !ships.Any() || !users.Any())
            {
                Console.WriteLine("Missing required data for events.");
                return;
            }

            var events = new List<Event>();
            var eventTypes = new[] { "Ship Arrival", "Ship Departure", "Container Loaded", "Container Unloaded", "Maintenance Alert", "Security Alert", "Weather Warning", "Delay Notification" };
            var categories = new[] { "Operations", "Security", "Maintenance", "Weather", "Traffic" };
            var priorities = new[] { "High", "Medium", "Low", "Critical" };
            var statuses = new[] { "New", "Acknowledged", "In Progress", "Resolved", "Closed" };

            for (int i = 0; i < 40; i++)
            {
                var random = new Random(i);
                var eventType = eventTypes[random.Next(eventTypes.Length)];
                var category = categories[random.Next(categories.Length)];
                var priority = priorities[random.Next(priorities.Length)];
                var status = statuses[random.Next(statuses.Length)];
                var container = random.Next(0, 3) > 0 ? containers[random.Next(containers.Count)] : null;
                var ship = random.Next(0, 3) > 0 ? ships[random.Next(ships.Count)] : null;
                var berth = random.Next(0, 4) > 0 ? berths[random.Next(berths.Count)] : null;
                var port = ports[random.Next(ports.Count)];
                var user = users[random.Next(users.Count)];
                var eventTime = DateTime.UtcNow.AddHours(-random.Next(1, 168)); // Last week

                events.Add(new Event
                {
                    EventType = eventType,
                    Category = category,
                    Priority = priority,
                    Status = status,
                    Title = GenerateEventTitle(eventType, ship?.Name, container?.ContainerNumber),
                    Description = GenerateEventDescription(eventType, ship?.Name, container?.ContainerNumber, berth?.Name),
                    Source = "Port Operations System",
                    ContainerId = container?.ContainerId,
                    ShipId = ship?.ShipId,
                    BerthId = berth?.BerthId,
                    PortId = port.PortId,
                    AssignedToUserId = random.Next(0, 3) > 0 ? user.UserId : null,
                    AcknowledgedByUserId = status != "New" ? user.UserId : null,
                    AcknowledgedAt = status != "New" ? eventTime.AddMinutes(random.Next(5, 60)) : null,
                    EventData = JsonSerializer.Serialize(new { 
                        severity = priority,
                        automated = random.Next(0, 2) == 0,
                        affectedSystems = new[] { "TOS", "CRANE_CONTROL", "SECURITY" }.Take(random.Next(1, 4))
                    }),
                    Metadata = JsonSerializer.Serialize(new {
                        sourceSystem = "MainframeOPS",
                        correlationId = Guid.NewGuid(),
                        processingTime = random.Next(100, 5000)
                    }),
                    Coordinates = ship?.Coordinates ?? berth?.Port?.Coordinates ?? port.Coordinates,
                    RequiresAction = priority == "High" || priority == "Critical",
                    IsResolved = status == "Resolved" || status == "Closed",
                    EventTimestamp = eventTime,
                    CreatedAt = eventTime,
                    UpdatedAt = status != "New" ? eventTime.AddMinutes(random.Next(30, 300)) : eventTime
                });
            }

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {events.Count} events.");
        }

        private static async Task SeedContainerMovements(ApplicationDbContext context)
        {
            if (await context.ContainerMovements.AnyAsync())
            {
                Console.WriteLine("Container movements already exist, skipping seeding...");
                return;
            }

            var containers = await context.Containers.ToListAsync();
            var ships = await context.Ships.ToListAsync();
            var berths = await context.Berths.ToListAsync();
            var ports = await context.Ports.ToListAsync();
            var users = await context.Users.ToListAsync();

            if (!containers.Any()) return;

            var movements = new List<ContainerMovement>();
            var movementTypes = new[] { "Load", "Unload", "Transfer", "Storage", "Inspection", "Delivery" };
            var statuses = new[] { "Planned", "In Progress", "Completed", "Delayed", "Cancelled" };
            var locations = new[] { "Yard A", "Yard B", "Ship Hold", "Truck Bay", "Rail Terminal", "Customs Area" };

            for (int i = 0; i < 60; i++)
            {
                var random = new Random(i);
                var container = containers[random.Next(containers.Count)];
                var movementType = movementTypes[random.Next(movementTypes.Length)];
                var status = statuses[random.Next(statuses.Length)];
                var fromLocation = locations[random.Next(locations.Length)];
                var toLocation = locations.Where(l => l != fromLocation).OrderBy(x => Guid.NewGuid()).First();
                var movementTime = DateTime.UtcNow.AddHours(-random.Next(1, 168));

                movements.Add(new ContainerMovement
                {
                    ContainerId = container.ContainerId,
                    MovementType = movementType,
                    FromLocation = fromLocation,
                    ToLocation = toLocation,
                    Status = status,
                    Coordinates = container.Coordinates,
                    EstimatedCompletion = movementTime.AddHours(random.Next(1, 8)),
                    ActualCompletion = status == "Completed" ? movementTime.AddHours(random.Next(1, 6)) : null,
                    DurationMinutes = status == "Completed" ? random.Next(30, 300) : null,
                    DistanceKm = random.Next(1, 50) / 10.0m,
                    ShipId = random.Next(0, 4) > 0 && ships.Any() ? ships[random.Next(ships.Count)].ShipId : null,
                    BerthId = random.Next(0, 4) > 0 && berths.Any() ? berths[random.Next(berths.Count)].BerthId : null,
                    PortId = ports.Any() ? ports[random.Next(ports.Count)].PortId : null,
                    Temperature = container.Temperature,
                    Humidity = random.Next(40, 80),
                    Notes = $"Movement from {fromLocation} to {toLocation}",
                    RecordedByUserId = users.Any() ? users[random.Next(users.Count)].UserId : null,
                    MovementTimestamp = movementTime,
                    CreatedAt = movementTime,
                    UpdatedAt = movementTime.AddMinutes(random.Next(0, 120))
                });
            }

            await context.ContainerMovements.AddRangeAsync(movements);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {movements.Count} container movements.");
        }

        private static async Task SeedAnalytics(ApplicationDbContext context)
        {
            if (await context.Analytics.AnyAsync())
            {
                Console.WriteLine("Analytics already exist, skipping analytics seeding...");
                return;
            }

            var ports = await context.Ports.ToListAsync();
            var berths = await context.Berths.ToListAsync();
            var ships = await context.Ships.ToListAsync();

            if (!ports.Any()) return;

            var analytics = new List<Analytics>();
            var metricTypes = new[] { "Container Throughput", "Ship Turnaround Time", "Berth Utilization", "Operational Efficiency", "Revenue", "Cost per TEU" };
            var categories = new[] { "Operations", "Financial", "Performance", "Capacity" };
            var periods = new[] { "Hourly", "Daily", "Weekly", "Monthly" };
            var trends = new[] { "Increasing", "Decreasing", "Stable", "Volatile" };
            var statuses = new[] { "Current", "Historical", "Projected" };

            for (int i = 0; i < 50; i++)
            {
                var random = new Random(i);
                var metricType = metricTypes[random.Next(metricTypes.Length)];
                var category = categories[random.Next(categories.Length)];
                var period = periods[random.Next(periods.Length)];
                var port = ports[random.Next(ports.Count)];
                var value = random.Next(100, 10000);
                var targetValue = value + random.Next(-200, 500);
                var previousValue = value + random.Next(-300, 300);
                var metricTime = DateTime.UtcNow.AddDays(-random.Next(0, 30));

                analytics.Add(new Analytics
                {
                    MetricType = metricType,
                    Category = category,
                    Period = period,
                    Value = value,
                    Unit = GetUnitForMetric(metricType),
                    TargetValue = targetValue,
                    PreviousPeriodValue = previousValue,
                    PercentageChange = previousValue > 0 ? ((value - previousValue) / (decimal)previousValue) * 100 : 0,
                    PortId = port.PortId,
                    BerthId = random.Next(0, 4) == 0 && berths.Any() ? berths[random.Next(berths.Count)].BerthId : null,
                    ShipId = random.Next(0, 6) == 0 && ships.Any() ? ships[random.Next(ships.Count)].ShipId : null,
                    Metadata = JsonSerializer.Serialize(new {
                        dataSource = "TOS",
                        calculation = "automated",
                        confidence = random.Next(85, 99),
                        methodology = "standard"
                    }),
                    Trend = trends[random.Next(trends.Length)],
                    Status = statuses[random.Next(statuses.Length)],
                    MetricTimestamp = metricTime,
                    CreatedAt = metricTime,
                    UpdatedAt = metricTime.AddHours(random.Next(1, 24))
                });
            }

            await context.Analytics.AddRangeAsync(analytics);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {analytics.Count} analytics records.");
        }

        private static string GenerateContainerId(int index)
        {
            var prefixes = new[] { "MSKU", "MSCU", "COSU", "EVGU", "HMMU", "OOLU", "CMAU", "COSB" };
            var random = new Random(index);
            var prefix = prefixes[random.Next(prefixes.Length)];
            return $"{prefix}{(1000000 + index):D7}";
        }

        private static string GenerateContainerNumber(int index)
        {
            var owners = new[] { "MSKU", "MSCU", "COSU", "EVGU" };
            var random = new Random(index);
            var owner = owners[random.Next(owners.Length)];
            var serial = (100000 + index).ToString();
            var checkDigit = CalculateCheckDigit(owner + serial);
            return $"{owner}{serial}{checkDigit}";
        }

        private static int CalculateCheckDigit(string containerNumber)
        {
            // ISO 6346 check digit calculation
            var values = "0123456789A?BCDEFGHIJK?LMNOPQRSTU?VWXYZ";
            var sum = 0;
            for (int i = 0; i < 10; i++)
            {
                var char_val = values.IndexOf(containerNumber[i]);
                sum += char_val * (int)Math.Pow(2, i);
            }
            return (sum % 11) % 10;
        }

        private static string GenerateEventTitle(string eventType, string? shipName, string? containerNumber)
        {
            return eventType switch
            {
                "Ship Arrival" => $"Ship {shipName ?? "Unknown"} arriving at berth",
                "Ship Departure" => $"Ship {shipName ?? "Unknown"} departing from port",
                "Container Loaded" => $"Container {containerNumber ?? "Unknown"} loaded successfully",
                "Container Unloaded" => $"Container {containerNumber ?? "Unknown"} unloaded from ship",
                "Maintenance Alert" => "Scheduled maintenance required for equipment",
                "Security Alert" => "Security incident detected in port area",
                "Weather Warning" => "Adverse weather conditions affecting operations",
                "Delay Notification" => "Operational delay affecting schedule",
                _ => $"{eventType} event notification"
            };
        }

        private static string GenerateEventDescription(string eventType, string? shipName, string? containerNumber, string? berthName)
        {
            return eventType switch
            {
                "Ship Arrival" => $"The vessel {shipName ?? "Unknown"} has arrived and is requesting berth assignment at {berthName ?? "available berth"}. All necessary preparations should be made for docking procedures.",
                "Ship Departure" => $"The vessel {shipName ?? "Unknown"} has completed operations and is departing from {berthName ?? "berth"}. Berth will be available for next assignment.",
                "Container Loaded" => $"Container {containerNumber ?? "Unknown"} has been successfully loaded onto the vessel. Loading operation completed without incidents.",
                "Container Unloaded" => $"Container {containerNumber ?? "Unknown"} has been unloaded from the vessel and is ready for further processing or pickup.",
                "Maintenance Alert" => $"Equipment at {berthName ?? "facility"} requires scheduled maintenance. Please coordinate with maintenance team to minimize operational impact.",
                "Security Alert" => "Unauthorized access detected in restricted area. Security team has been notified and is responding to the incident.",
                "Weather Warning" => "Weather conditions are deteriorating and may affect crane operations and vessel movements. Monitor conditions closely.",
                "Delay Notification" => "Operational delays have been detected that may impact the current schedule. Please review and adjust plans accordingly.",
                _ => $"System generated {eventType} notification requiring attention."
            };
        }

        private static string GetUnitForMetric(string metricType)
        {
            return metricType switch
            {
                "Container Throughput" => "TEU",
                "Ship Turnaround Time" => "Hours",
                "Berth Utilization" => "Percent",
                "Operational Efficiency" => "Percent",
                "Revenue" => "USD",
                "Cost per TEU" => "USD",
                _ => "Units"
            };
        }
    }
}