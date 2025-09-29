using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Seeding
{
    /// <summary>
    /// Seeds extensive sample data into the database for testing purposes
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds the database with comprehensive sample data
        /// </summary>
        /// <param name="context">The database context</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure the database is created
            await context.Database.EnsureCreatedAsync();

            // Check if we need to seed each entity type separately
            await SeedPorts(context);
            await SeedBerths(context);
            await SeedShips(context);
            await SeedContainers(context);
            await SeedBerthAssignments(context);
            await SeedShipContainers(context);

            Console.WriteLine("Sample data seeding completed successfully!");
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
                    Name = "Port of Copenhagen",
                    Location = "Copenhagen, Denmark",
                    TotalContainerCapacity = 10000
                },
                new Port
                {
                    Name = "Port of Rotterdam",
                    Location = "Rotterdam, Netherlands", 
                    TotalContainerCapacity = 15000
                },
                new Port
                {
                    Name = "Port of Hamburg",
                    Location = "Hamburg, Germany",
                    TotalContainerCapacity = 12000
                },
                new Port
                {
                    Name = "Port of Antwerp",
                    Location = "Antwerp, Belgium",
                    TotalContainerCapacity = 8000
                },
                new Port
                {
                    Name = "Port of Gothenburg",
                    Location = "Gothenburg, Sweden",
                    TotalContainerCapacity = 6500
                },
                new Port
                {
                    Name = "Port of Oslo",
                    Location = "Oslo, Norway",
                    TotalContainerCapacity = 4000
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
                Console.WriteLine("No ports found, cannot seed berths. Please seed ports first.");
                return;
            }

            var berths = new List<Berth>();

            // Add berths for each existing port
            foreach (var port in ports)
            {
                // Generate berths based on port name patterns
                if (port.Name.Contains("Copenhagen"))
                {
                    berths.AddRange(new[]
                    {
                        new Berth { PortId = port.PortId, Name = "CPH-A1", Capacity = 500, Status = "Available" },
                        new Berth { PortId = port.PortId, Name = "CPH-A2", Capacity = 750, Status = "Occupied" },
                        new Berth { PortId = port.PortId, Name = "CPH-B1", Capacity = 600, Status = "Available" },
                        new Berth { PortId = port.PortId, Name = "CPH-B2", Capacity = 800, Status = "Maintenance" },
                        new Berth { PortId = port.PortId, Name = "CPH-C1", Capacity = 550, Status = "Available" }
                    });
                }
                else if (port.Name.Contains("Rotterdam"))
                {
                    berths.AddRange(new[]
                    {
                        new Berth { PortId = port.PortId, Name = "RTM-1", Capacity = 1000, Status = "Occupied" },
                        new Berth { PortId = port.PortId, Name = "RTM-2", Capacity = 1200, Status = "Available" },
                        new Berth { PortId = port.PortId, Name = "RTM-3", Capacity = 900, Status = "Occupied" },
                        new Berth { PortId = port.PortId, Name = "RTM-4", Capacity = 1100, Status = "Available" },
                        new Berth { PortId = port.PortId, Name = "RTM-5", Capacity = 850, Status = "Available" }
                    });
                }
                else if (port.Name.Contains("Hamburg"))
                {
                    berths.AddRange(new[]
                    {
                        new Berth { PortId = port.PortId, Name = "HAM-North-1", Capacity = 800, Status = "Available" },
                        new Berth { PortId = port.PortId, Name = "HAM-North-2", Capacity = 750, Status = "Occupied" },
                        new Berth { PortId = port.PortId, Name = "HAM-South-1", Capacity = 900, Status = "Available" },
                        new Berth { PortId = port.PortId, Name = "HAM-South-2", Capacity = 700, Status = "Maintenance" },
                        new Berth { PortId = port.PortId, Name = "HAM-East-1", Capacity = 650, Status = "Available" }
                    });
                }
                else
                {
                    // Default berths for any other port
                    var random = new Random(port.PortId);
                    var portCode = port.Name.Substring(0, Math.Min(3, port.Name.Length)).ToUpper();
                    for (int i = 1; i <= 3; i++)
                    {
                        berths.Add(new Berth 
                        { 
                            PortId = port.PortId, 
                            Name = $"{portCode}-{i}", 
                            Capacity = 300 + random.Next(200, 500), 
                            Status = i % 2 == 0 ? "Available" : "Occupied" 
                        });
                    }
                }
            }

            if (berths.Any())
            {
                await context.Berths.AddRangeAsync(berths);
                await context.SaveChangesAsync();
                Console.WriteLine($"Seeded {berths.Count} berths across {ports.Count} ports.");
            }
        }

        private static async Task SeedShips(ApplicationDbContext context)
        {
            if (await context.Ships.AnyAsync())
            {
                Console.WriteLine("Ships already exist, skipping ship seeding...");
                return;
            }

            var ships = new List<Ship>
            {
                new Ship { Name = "Maersk Edinburgh", Status = "Docked" },
                new Ship { Name = "MSC Gulsun", Status = "At Sea" },
                new Ship { Name = "COSCO Shipping Universe", Status = "Loading" },
                new Ship { Name = "Ever Given", Status = "Docked" },
                new Ship { Name = "HMM Algeciras", Status = "At Sea" },
                new Ship { Name = "MSC Mia", Status = "Loading" },
                new Ship { Name = "OOCL Hong Kong", Status = "Docked" },
                new Ship { Name = "Madrid Maersk", Status = "At Sea" },
                new Ship { Name = "CMA CGM Antoine de Saint Exupery", Status = "Loading" },
                new Ship { Name = "MSC Oscar", Status = "Docked" },
                new Ship { Name = "COSCO Shipping Aries", Status = "At Sea" },
                new Ship { Name = "Ever Ace", Status = "Maintenance" }
            };

            await context.Ships.AddRangeAsync(ships);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {ships.Count} ships.");
        }

        private static async Task SeedContainers(ApplicationDbContext context)
        {
            if (await context.Containers.AnyAsync())
            {
                Console.WriteLine("Containers already exist, skipping container seeding...");
                return;
            }

            var ships = await context.Ships.ToListAsync();
            var baseTime = DateTime.UtcNow;
            var containers = new List<Container>();

            // Container types and statuses for variety
            var containerTypes = new[] { "Dry", "Refrigerated", "Hazardous", "Liquid", "Open Top", "Flat Rack" };
            var containerStatuses = new[] { "Empty", "Loaded", "In Transit", "Loading", "Unloading", "Inspected" };
            var locations = new[] { 
                "Port of Copenhagen", "Port of Rotterdam", "Port of Hamburg", 
                "Port of Antwerp", "Port of Gothenburg", "Port of Oslo",
                "At Sea", "Warehouse A", "Warehouse B", "Customs Area"
            };

            // Generate containers with realistic data
            for (int i = 1; i <= 50; i++)
            {
                var random = new Random(i); // Deterministic for consistent data
                var ship = random.Next(0, 4) == 0 ? null : ships[random.Next(ships.Count)]; // 25% chance of no ship
                
                containers.Add(new Container
                {
                    ContainerId = GenerateContainerId(i),
                    Name = GenerateContainerName(i),
                    Type = containerTypes[random.Next(containerTypes.Length)],
                    Status = containerStatuses[random.Next(containerStatuses.Length)],
                    CurrentLocation = locations[random.Next(locations.Length)],
                    CreatedAt = baseTime.AddDays(-random.Next(1, 30)),
                    UpdatedAt = baseTime.AddHours(-random.Next(1, 48)),
                    ShipId = ship?.ShipId
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
                Console.WriteLine("Berth assignments already exist, skipping berth assignment seeding...");
                return;
            }

            var containers = await context.Containers.ToListAsync();
            var berths = await context.Berths.ToListAsync();
            var baseTime = DateTime.UtcNow;
            var assignments = new List<BerthAssignment>();

            var random = new Random(42); // Deterministic seed

            // Create 20 berth assignments (mix of current and historical)
            for (int i = 0; i < 20; i++)
            {
                var container = containers[random.Next(containers.Count)];
                var berth = berths[random.Next(berths.Count)];
                var assignedTime = baseTime.AddDays(-random.Next(1, 10));
                var shouldBeReleased = random.Next(0, 3) > 0; // 66% chance of being released

                assignments.Add(new BerthAssignment
                {
                    ContainerId = container.ContainerId,
                    BerthId = berth.BerthId,
                    AssignedAt = assignedTime,
                    ReleasedAt = shouldBeReleased ? assignedTime.AddHours(random.Next(2, 24)) : null
                });
            }

            await context.BerthAssignments.AddRangeAsync(assignments);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {assignments.Count} berth assignments.");
        }

        private static async Task SeedShipContainers(ApplicationDbContext context)
        {
            if (await context.ShipContainers.AnyAsync())
            {
                Console.WriteLine("Ship containers already exist, skipping ship container seeding...");
                return;
            }

            var containers = await context.Containers.Where(c => c.ShipId != null).ToListAsync();
            var ships = await context.Ships.ToListAsync();
            var baseTime = DateTime.UtcNow;
            var shipContainers = new List<ShipContainer>();

            var random = new Random(123); // Deterministic seed

            // Create ship container operations for containers that are on ships
            foreach (var container in containers.Take(15)) // Limit to 15 operations
            {
                if (container.ShipId.HasValue)
                {
                    shipContainers.Add(new ShipContainer
                    {
                        ShipId = container.ShipId.Value,
                        ContainerId = container.ContainerId,
                        LoadedAt = baseTime.AddDays(-random.Next(1, 15))
                    });
                }
            }

            await context.ShipContainers.AddRangeAsync(shipContainers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {shipContainers.Count} ship container operations.");
        }

        private static string GenerateContainerId(int index)
        {
            var prefixes = new[] { "MSKU", "MSCU", "COSU", "EVGU", "HMMU", "OOLU", "CMAU", "COSB" };
            var random = new Random(index);
            var prefix = prefixes[random.Next(prefixes.Length)];
            return $"{prefix}{(1000000 + index):D7}";
        }

        private static string GenerateContainerName(int index)
        {
            var names = new[]
            {
                "Electronics Container", "Automotive Parts", "Refrigerated Goods", "Textile Products",
                "Chemical Products", "Food & Beverages", "Machinery Equipment", "Construction Materials",
                "Pharmaceuticals", "Consumer Goods", "Raw Materials", "Agricultural Products",
                "Industrial Equipment", "Medical Supplies", "Furniture & Fixtures", "Paper Products",
                "Metal Components", "Plastic Materials", "Glass Products", "Rubber Materials",
                "Oil & Gas Equipment", "Mining Equipment", "Renewable Energy Parts", "Technology Hardware",
                "Clothing & Apparel", "Sports Equipment", "Musical Instruments", "Art & Antiques",
                "Books & Media", "Toys & Games", "Beauty Products", "Cleaning Supplies",
                "Office Supplies", "Tools & Hardware", "Garden Supplies", "Pet Products",
                "Home Appliances", "Kitchen Equipment", "Bathroom Fixtures", "Lighting Equipment",
                "Security Systems", "Communication Devices", "Laboratory Equipment", "Educational Materials",
                "Travel Accessories", "Outdoor Gear", "Fitness Equipment", "Health Products",
                "Luxury Goods", "Seasonal Items"
            };
            
            var random = new Random(index);
            return names[random.Next(names.Length)];
        }
    }
}