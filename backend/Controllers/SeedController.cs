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
                _logger.LogInformation("Starting production data seeding...");
                await ProductionDataSeeder.SeedAsync(_context, _logger);
                _logger.LogInformation("Production data seeding completed successfully!");
                
                return Ok(new { 
                    success = true, 
                    message = "Production data seeding completed successfully!",
                    data = new {
                        roles = "4 user roles (Admin, PortManager, Operator, Viewer)",
                        users = "4 test users with proper role assignments",
                        ports = "3 major ports with realistic data",
                        berths = "15 berths across all ports",
                        ships = "3 ships with proper specifications",
                        containers = "100 containers with diverse cargo",
                        assignments = "Sample berth assignments"
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