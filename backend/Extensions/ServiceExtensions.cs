using Backend.Data;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Seeds the database with production data if it's empty
        /// </summary>
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    
                    // Use comprehensive seeding service instead of simple one
                    var comprehensiveSeeding = services.GetRequiredService<ComprehensiveDataSeedingService>();
                    await comprehensiveSeeding.SeedAllAsync();
                    
                    logger.LogInformation("Database seeding completed successfully");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
        }
    }
}