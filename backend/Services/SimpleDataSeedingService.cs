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

        // Change method signature to accept isProduction parameter
        public async Task SeedAllDataAsync(bool isProduction = false)
        {
            _logger.LogInformation("Starting simple data seeding...");
            
            // Just ensure database is created - no complex seeding for now
            await _context.Database.EnsureCreatedAsync();
            
            _logger.LogInformation("Simple data seeding completed");
        }
    }
}