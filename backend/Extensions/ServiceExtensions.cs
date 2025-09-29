using Backend.Data;
using Backend.Data.Seeding;
using Microsoft.EntityFrameworkCore;

namespace Backend.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Seeds the database with initial data if it's empty
        /// </summary>
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    await DataSeeder.SeedAsync(context);
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