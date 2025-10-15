using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class SimpleDataSeedingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SimpleDataSeedingService> _logger;

        public SimpleDataSeedingService(
            ApplicationDbContext context,
            ILogger<SimpleDataSeedingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAllDataAsync(bool isProduction = false)
        {
            _logger.LogInformation("Starting simple data seeding...");
            
            // Ensure database and tables are created
            await _context.Database.EnsureCreatedAsync();
            
            _logger.LogInformation("Simple data seeding completed - database and tables ensured");
        }
    }
}