using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Seeding
{
    /// <summary>
    /// Enhanced data seeder with comprehensive business data for testing and demos
    /// </summary>
    public static class EnhancedDataSeeder
    {
        /// <summary>
        /// Seeds the database with comprehensive business sample data
        /// </summary>
        /// <param name="context">The database context</param>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            Console.WriteLine("🌱 Starting Enhanced Business Data Seeding...");
            
            // Check if we need to seed each entity type separately
            await SeedPortsAsync(context);
            await SeedBerthsAsync(context);
            await SeedShipsAsync(context);
            await SeedContainersAsync(context);
            await SeedBerthAssignmentsAsync(context);
            await SeedShipContainersAsync(context);

            Console.WriteLine("✅ Enhanced Business Data Seeding completed successfully!");
        }

        private static async Task SeedPortsAsync(ApplicationDbContext context)
        {
            if (await context.Ports.AnyAsync())
            {
                Console.WriteLine("Ports already exist, skipping enhanced port seeding...");
                return;
            }

            var ports = new List<Port>
            {
                // Major European Ports
                new Port { Name = "Port of Copenhagen", Location = "Copenhagen, Denmark", TotalContainerCapacity = 10000 },
                new Port { Name = "Port of Rotterdam", Location = "Rotterdam, Netherlands", TotalContainerCapacity = 25000 },
                new Port { Name = "Port of Hamburg", Location = "Hamburg, Germany", TotalContainerCapacity = 20000 },
                new Port { Name = "Port of Antwerp", Location = "Antwerp, Belgium", TotalContainerCapacity = 18000 },
                new Port { Name = "Port of Gothenburg", Location = "Gothenburg, Sweden", TotalContainerCapacity = 8500 },
                new Port { Name = "Port of Oslo", Location = "Oslo, Norway", TotalContainerCapacity = 6000 },
                new Port { Name = "Port of Valencia", Location = "Valencia, Spain", TotalContainerCapacity = 14000 },
                new Port { Name = "Port of Genoa", Location = "Genoa, Italy", TotalContainerCapacity = 11000 },
                new Port { Name = "Port of Southampton", Location = "Southampton, UK", TotalContainerCapacity = 9500 },
                new Port { Name = "Port of Le Havre", Location = "Le Havre, France", TotalContainerCapacity = 13000 },
                
                // Major Asian Ports
                new Port { Name = "Port of Shanghai", Location = "Shanghai, China", TotalContainerCapacity = 45000 },
                new Port { Name = "Port of Singapore", Location = "Singapore", TotalContainerCapacity = 35000 },
                new Port { Name = "Port of Busan", Location = "Busan, South Korea", TotalContainerCapacity = 22000 },
                new Port { Name = "Port of Hong Kong", Location = "Hong Kong", TotalContainerCapacity = 18000 },
                new Port { Name = "Port of Tokyo", Location = "Tokyo, Japan", TotalContainerCapacity = 16000 },
                new Port { Name = "Port of Kaohsiung", Location = "Kaohsiung, Taiwan", TotalContainerCapacity = 12000 },
                
                // American Ports
                new Port { Name = "Port of Los Angeles", Location = "Los Angeles, USA", TotalContainerCapacity = 28000 },
                new Port { Name = "Port of Long Beach", Location = "Long Beach, USA", TotalContainerCapacity = 25000 },
                new Port { Name = "Port of New York", Location = "New York, USA", TotalContainerCapacity = 20000 },
                new Port { Name = "Port of Miami", Location = "Miami, USA", TotalContainerCapacity = 15000 },
                new Port { Name = "Port of Vancouver", Location = "Vancouver, Canada", TotalContainerCapacity = 12000 },
                new Port { Name = "Port of Santos", Location = "Santos, Brazil", TotalContainerCapacity = 14000 },
                
                // Middle Eastern & African Ports
                new Port { Name = "Port of Dubai", Location = "Dubai, UAE", TotalContainerCapacity = 30000 },
                new Port { Name = "Port of Suez", Location = "Suez, Egypt", TotalContainerCapacity = 12000 },
                new Port { Name = "Port of Durban", Location = "Durban, South Africa", TotalContainerCapacity = 8000 },
                new Port { Name = "Port of Jeddah", Location = "Jeddah, Saudi Arabia", TotalContainerCapacity = 10000 }
            };

            await context.Ports.AddRangeAsync(ports);
            await context.SaveChangesAsync();
            Console.WriteLine($"🏗️ Seeded {ports.Count} major world ports.");
        }

        private static async Task SeedBerthsAsync(ApplicationDbContext context)
        {
            if (await context.Berths.AnyAsync())
            {
                Console.WriteLine("Berths already exist, skipping enhanced berth seeding...");
                return;
            }

            var ports = await context.Ports.ToListAsync();
            var berths = new List<Berth>();
            var random = new Random(42);

            foreach (var port in ports)
            {
                // Calculate number of berths based on port capacity
                int berthCount = Math.Max(4, port.TotalContainerCapacity / 2000);
                string portCode = GetPortCode(port.Name);

                for (int i = 1; i <= berthCount; i++)
                {
                    berths.Add(new Berth
                    {
                        PortId = port.PortId,
                        Name = $"{portCode}-{i:D2}",
                        Capacity = 300 + random.Next(200, 800),
                        Status = GetRandomBerthStatus(random)
                    });
                }
            }

            await context.Berths.AddRangeAsync(berths);
            await context.SaveChangesAsync();
            Console.WriteLine($"⚓ Seeded {berths.Count} berths across {ports.Count} ports.");
        }

        private static async Task SeedShipsAsync(ApplicationDbContext context)
        {
            if (await context.Ships.AnyAsync())
            {
                Console.WriteLine("Ships already exist, skipping enhanced ship seeding...");
                return;
            }

            var ships = new List<Ship>
            {
                // Maersk Fleet
                new Ship { Name = "Maersk Edinburgh", Status = "Docked" },
                new Ship { Name = "Madrid Maersk", Status = "At Sea" },
                new Ship { Name = "Maersk Honam", Status = "Loading" },
                new Ship { Name = "Maersk Kleven", Status = "Docked" },
                new Ship { Name = "Maersk Eindhoven", Status = "At Sea" },
                new Ship { Name = "Maersk Essen", Status = "Maintenance" },
                new Ship { Name = "Maersk Sevilla", Status = "Loading" },
                new Ship { Name = "Maersk Halifax", Status = "Docked" },
                
                // MSC Fleet
                new Ship { Name = "MSC Gulsun", Status = "At Sea" },
                new Ship { Name = "MSC Mia", Status = "Loading" },
                new Ship { Name = "MSC Oscar", Status = "Docked" },
                new Ship { Name = "MSC Zoe", Status = "At Sea" },
                new Ship { Name = "MSC Maya", Status = "Loading" },
                new Ship { Name = "MSC Diana", Status = "Docked" },
                new Ship { Name = "MSC Lucinda", Status = "At Sea" },
                new Ship { Name = "MSC Emma", Status = "Loading" },
                
                // COSCO Fleet
                new Ship { Name = "COSCO Shipping Universe", Status = "Loading" },
                new Ship { Name = "COSCO Shipping Aries", Status = "At Sea" },
                new Ship { Name = "COSCO Shipping Galaxy", Status = "Docked" },
                new Ship { Name = "COSCO Shipping Taurus", Status = "Loading" },
                new Ship { Name = "COSCO Shipping Leo", Status = "At Sea" },
                new Ship { Name = "COSCO Shipping Pisces", Status = "Docked" },
                
                // Evergreen Fleet
                new Ship { Name = "Ever Given", Status = "Docked" },
                new Ship { Name = "Ever Ace", Status = "Maintenance" },
                new Ship { Name = "Ever Globe", Status = "At Sea" },
                new Ship { Name = "Ever Golden", Status = "Loading" },
                new Ship { Name = "Ever Goods", Status = "Docked" },
                new Ship { Name = "Ever Goal", Status = "At Sea" },
                new Ship { Name = "Ever Greet", Status = "Loading" },
                
                // HMM Fleet
                new Ship { Name = "HMM Algeciras", Status = "At Sea" },
                new Ship { Name = "HMM Oslo", Status = "Loading" },
                new Ship { Name = "HMM Rotterdam", Status = "Docked" },
                new Ship { Name = "HMM Hamburg", Status = "At Sea" },
                new Ship { Name = "HMM Copenhagen", Status = "Loading" },
                
                // OOCL Fleet
                new Ship { Name = "OOCL Hong Kong", Status = "Docked" },
                new Ship { Name = "OOCL Shanghai", Status = "At Sea" },
                new Ship { Name = "OOCL Shenzhen", Status = "Loading" },
                new Ship { Name = "OOCL Singapore", Status = "Docked" },
                new Ship { Name = "OOCL Tokyo", Status = "At Sea" },
                
                // CMA CGM Fleet
                new Ship { Name = "CMA CGM Antoine de Saint Exupery", Status = "Loading" },
                new Ship { Name = "CMA CGM Marco Polo", Status = "At Sea" },
                new Ship { Name = "CMA CGM Alexander von Humboldt", Status = "Docked" },
                new Ship { Name = "CMA CGM Bougainville", Status = "Loading" },
                new Ship { Name = "CMA CGM Jacques Saade", Status = "At Sea" },
                
                // ONE Fleet
                new Ship { Name = "ONE Stork", Status = "At Sea" },
                new Ship { Name = "ONE Trust", Status = "Docked" },
                new Ship { Name = "ONE Innovation", Status = "Loading" },
                new Ship { Name = "ONE Commitment", Status = "At Sea" },
                new Ship { Name = "ONE Magnificence", Status = "Docked" },
                
                // Hapag-Lloyd Fleet
                new Ship { Name = "Hapag-Lloyd Berlin Express", Status = "Docked" },
                new Ship { Name = "Hapag-Lloyd Bonn Express", Status = "At Sea" },
                new Ship { Name = "Hapag-Lloyd Bremen Express", Status = "Loading" },
                new Ship { Name = "Hapag-Lloyd Hamburg Express", Status = "At Sea" },
                
                // Yang Ming Fleet
                new Ship { Name = "YM Worth", Status = "At Sea" },
                new Ship { Name = "YM Wealth", Status = "Docked" },
                new Ship { Name = "YM Wonder", Status = "Loading" },
                new Ship { Name = "YM Wisdom", Status = "At Sea" },
                
                // ZIM Fleet
                new Ship { Name = "ZIM Integrated", Status = "Docked" },
                new Ship { Name = "ZIM Kingston", Status = "Loading" },
                new Ship { Name = "ZIM Monaco", Status = "At Sea" },
                
                // Additional Regional Ships
                new Ship { Name = "Atlantic Pioneer", Status = "Docked" },
                new Ship { Name = "Pacific Navigator", Status = "At Sea" },
                new Ship { Name = "Mediterranean Star", Status = "Loading" },
                new Ship { Name = "Baltic Explorer", Status = "Docked" },
                new Ship { Name = "Nordic Spirit", Status = "At Sea" }
            };

            await context.Ships.AddRangeAsync(ships);
            await context.SaveChangesAsync();
            Console.WriteLine($"🚢 Seeded {ships.Count} ships from major shipping lines.");
        }

        private static async Task SeedContainersAsync(ApplicationDbContext context)
        {
            if (await context.Containers.AnyAsync())
            {
                Console.WriteLine("Containers already exist, skipping enhanced container seeding...");
                return;
            }

            var ships = await context.Ships.ToListAsync();
            var ports = await context.Ports.ToListAsync();
            var baseTime = DateTime.UtcNow;
            var containers = new List<Container>();

            var containerTypes = new[] { "Dry", "Refrigerated", "Hazardous", "Liquid", "Open Top", "Flat Rack", "Tank", "ISO Tank", "High Cube", "Standard" };
            var containerStatuses = new[] { "Empty", "Loaded", "In Transit", "Loading", "Unloading", "Inspected", "Damaged", "Under Repair", "Quarantine", "Available", "Reserved", "Customs Hold" };

            // Generate 300 containers with realistic distribution
            for (int i = 1; i <= 300; i++)
            {
                var random = new Random(i);
                var ship = random.Next(0, 5) == 0 ? null : ships[random.Next(ships.Count)]; // 20% chance of no ship
                var port = ports[random.Next(ports.Count)];

                containers.Add(new Container
                {
                    ContainerId = GenerateContainerId(i),
                    Name = GenerateContainerName(i),
                    Type = containerTypes[random.Next(containerTypes.Length)],
                    Status = containerStatuses[random.Next(containerStatuses.Length)],
                    CurrentLocation = GetContainerLocation(random, port.Name),
                    CreatedAt = baseTime.AddDays(-random.Next(1, 180)), // Up to 6 months old
                    UpdatedAt = baseTime.AddHours(-random.Next(1, 336)), // Up to 2 weeks ago
                    ShipId = ship?.ShipId
                });
            }

            await context.Containers.AddRangeAsync(containers);
            await context.SaveChangesAsync();
            Console.WriteLine($"📦 Seeded {containers.Count} containers with diverse cargo types.");
        }

        private static async Task SeedBerthAssignmentsAsync(ApplicationDbContext context)
        {
            if (await context.BerthAssignments.AnyAsync())
            {
                Console.WriteLine("Berth assignments already exist, skipping enhanced berth assignment seeding...");
                return;
            }

            var containers = await context.Containers.ToListAsync();
            var berths = await context.Berths.ToListAsync();
            var baseTime = DateTime.UtcNow;
            var assignments = new List<BerthAssignment>();
            var random = new Random(42);

            // Create 120 berth assignments for better coverage
            for (int i = 0; i < Math.Min(120, containers.Count); i++)
            {
                var container = containers[i];
                var berth = berths[random.Next(berths.Count)];
                var assignedTime = baseTime.AddDays(-random.Next(1, 90)).AddHours(-random.Next(0, 24));
                var shouldBeReleased = random.Next(0, 4) > 0; // 75% chance of being released

                assignments.Add(new BerthAssignment
                {
                    ContainerId = container.ContainerId,
                    BerthId = berth.BerthId,
                    AssignedAt = assignedTime,
                    ReleasedAt = shouldBeReleased ? assignedTime.AddHours(random.Next(2, 168)) : null // Up to 1 week
                });
            }

            await context.BerthAssignments.AddRangeAsync(assignments);
            await context.SaveChangesAsync();
            Console.WriteLine($"⚓ Seeded {assignments.Count} berth assignments with realistic timelines.");
        }

        private static async Task SeedShipContainersAsync(ApplicationDbContext context)
        {
            if (await context.ShipContainers.AnyAsync())
            {
                Console.WriteLine("Ship containers already exist, skipping enhanced ship container seeding...");
                return;
            }

            var containers = await context.Containers.Where(c => c.ShipId != null).ToListAsync();
            var baseTime = DateTime.UtcNow;
            var shipContainers = new List<ShipContainer>();
            var random = new Random(123);

            // Create ship container operations for containers that are on ships
            foreach (var container in containers.Take(80)) // Up to 80 operations
            {
                if (container.ShipId.HasValue)
                {
                    shipContainers.Add(new ShipContainer
                    {
                        ShipId = container.ShipId.Value,
                        ContainerId = container.ContainerId,
                        LoadedAt = baseTime.AddDays(-random.Next(1, 30)) // Loaded within last month
                    });
                }
            }

            await context.ShipContainers.AddRangeAsync(shipContainers);
            await context.SaveChangesAsync();
            Console.WriteLine($"🚢📦 Seeded {shipContainers.Count} ship-container operations.");
        }

        #region Helper Methods

        private static string GetPortCode(string portName)
        {
            return portName switch
            {
                "Port of Copenhagen" => "CPH",
                "Port of Rotterdam" => "RTM",
                "Port of Hamburg" => "HAM",
                "Port of Antwerp" => "ANR",
                "Port of Gothenburg" => "GOT",
                "Port of Oslo" => "OSL",
                "Port of Valencia" => "VLC",
                "Port of Genoa" => "GOA",
                "Port of Southampton" => "SOU",
                "Port of Le Havre" => "LEH",
                "Port of Shanghai" => "SHA",
                "Port of Singapore" => "SIN",
                "Port of Busan" => "BUS",
                "Port of Hong Kong" => "HKG",
                "Port of Tokyo" => "TYO",
                "Port of Kaohsiung" => "KAO",
                "Port of Los Angeles" => "LAX",
                "Port of Long Beach" => "LGB",
                "Port of New York" => "NYC",
                "Port of Miami" => "MIA",
                "Port of Vancouver" => "VAN",
                "Port of Santos" => "SAN",
                "Port of Dubai" => "DXB",
                "Port of Suez" => "SUZ",
                "Port of Durban" => "DUR",
                "Port of Jeddah" => "JED",
                _ => "PRT"
            };
        }

        private static string GetRandomBerthStatus(Random random)
        {
            var statuses = new[] { "Available", "Occupied", "Maintenance", "Reserved" };
            return statuses[random.Next(statuses.Length)];
        }

        private static string GetContainerLocation(Random random, string portName)
        {
            var locations = new[]
            {
                portName,
                $"{portName} - Warehouse A",
                $"{portName} - Warehouse B",
                $"{portName} - Customs Area",
                $"{portName} - Storage Yard",
                $"{portName} - Inspection Zone",
                "At Sea - Atlantic",
                "At Sea - Pacific",
                "At Sea - Mediterranean",
                "Transit Hub",
                "Distribution Center"
            };
            return locations[random.Next(locations.Length)];
        }

        private static string GenerateContainerId(int index)
        {
            var prefixes = new[] { "MSKU", "MSCU", "COSU", "EVGU", "HMMU", "OOLU", "CMAU", "COSB", "ONEU", "HLCU", "ZIMU", "YMLU" };
            var random = new Random(index);
            var prefix = prefixes[random.Next(prefixes.Length)];
            return $"{prefix}{(1000000 + index):D7}";
        }

        private static string GenerateContainerName(int index)
        {
            var cargoTypes = new[]
            {
                "Electronics Container", "Automotive Parts", "Refrigerated Goods", "Textile Products",
                "Chemical Products", "Food & Beverages", "Machinery Equipment", "Construction Materials",
                "Pharmaceuticals", "Consumer Goods", "Raw Materials", "Agricultural Products",
                "Industrial Equipment", "Medical Supplies", "Furniture & Fixtures", "Paper Products",
                "Metal Components", "Plastic Materials", "Glass Products", "Rubber Materials",
                "Oil & Gas Equipment", "Mining Equipment", "Renewable Energy Parts", "Technology Hardware",
                "Clothing & Apparel", "Sports Equipment", "Musical Instruments", "Art & Antiques",
                "Luxury Goods", "Frozen Foods", "Dairy Products", "Grains & Cereals",
                "Coffee & Tea", "Wine & Spirits", "Timber & Wood", "Coal & Minerals",
                "Fertilizers", "Steel Products", "Aluminum Goods", "Copper Materials",
                "Solar Panels", "Wind Turbine Parts", "Electric Vehicle Components", "Batteries",
                "Computer Hardware", "Mobile Devices", "Gaming Equipment", "Home Appliances",
                "Beauty Products", "Personal Care", "Baby Products", "Pet Supplies"
            };

            var random = new Random(index);
            return cargoTypes[random.Next(cargoTypes.Length)];
        }

        #endregion
    }
}