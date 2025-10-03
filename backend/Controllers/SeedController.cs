using Backend.Data;
using Backend.Data.Seeding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(ApplicationDbContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Trigger enhanced business data seeding
        /// </summary>
        /// <returns>Seeding result</returns>
        [HttpPost("enhanced-business-data")]
        public async Task<IActionResult> SeedEnhancedBusinessData()
        {
            try
            {
                _logger.LogInformation("Starting enhanced business data seeding...");
                await EnhancedDataSeeder.SeedAsync(_context);
                _logger.LogInformation("Enhanced business data seeding completed successfully!");
                
                return Ok(new { 
                    success = true, 
                    message = "Enhanced business data seeding completed successfully!",
                    data = new {
                        ports = "25 major world ports",
                        ships = "60+ ships from major shipping lines", 
                        containers = "300 containers with diverse cargo",
                        berthAssignments = "120+ berth assignments",
                        shipContainers = "80+ ship-container operations"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during enhanced business data seeding");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error occurred during seeding", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Get seeding status and data counts
        /// </summary>
        /// <returns>Current data status</returns>
        [HttpGet("status")]
        public async Task<IActionResult> GetSeedingStatus()
        {
            try
            {
                var status = new
                {
                    ports = await _context.Ports.CountAsync(),
                    ships = await _context.Ships.CountAsync(),
                    containers = await _context.Containers.CountAsync(),
                    berths = await _context.Berths.CountAsync(),
                    berthAssignments = await _context.BerthAssignments.CountAsync(),
                    shipContainers = await _context.ShipContainers.CountAsync(),
                    users = await _context.Users.CountAsync(),
                    roles = await _context.Roles.CountAsync()
                };

                return Ok(new { success = true, data = status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking seeding status");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Error occurred while checking status", 
                    error = ex.Message 
                });
            }
        }
    }
}