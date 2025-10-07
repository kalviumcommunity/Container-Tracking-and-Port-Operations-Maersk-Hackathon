using Backend.Attributes;
using Backend.Data;
using Backend.Data.Seeding;
using Backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
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
        /// Trigger enhanced business data seeding (Admin only)
        /// </summary>
        /// <returns>Seeding result</returns>
        [HttpPost("enhanced-business-data")]
        [RequirePermission("ManageUsers")] // Only admins can seed data
        public async Task<IActionResult> SeedEnhancedBusinessData()
        {
            try
            {
                _logger.LogInformation("Starting enhanced business data seeding...");
                await DataSeeder.SeedAsync(_context);
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
        /// Get seeding status and data counts (Admin only)
        /// </summary>
        /// <returns>Current data status</returns>
        [HttpGet("status")]
        [RequirePermission("ManageUsers")] // Only admins can view seeding status
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