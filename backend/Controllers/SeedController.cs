using Backend.Attributes;
using Backend.Data;
using Backend.Services;
using Backend.DTOs;
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
        private readonly SimpleDataSeedingService _simpleSeedingService;
        private readonly ComprehensiveDataSeedingService _comprehensiveDataSeedingService;
        private readonly ILogger<SeedController> _logger;

        public SeedController(
            ApplicationDbContext context, 
            SimpleDataSeedingService simpleSeedingService,
            ComprehensiveDataSeedingService comprehensiveDataSeedingService,
            ILogger<SeedController> logger)
        {
            _context = context;
            _simpleSeedingService = simpleSeedingService;
            _comprehensiveDataSeedingService = comprehensiveDataSeedingService;
            _logger = logger;
        }

        /// <summary>
        /// Trigger simple data seeding
        /// </summary>
        /// <returns>Seeding result</returns>
        [HttpPost("simple")]
        [RequirePermission("SeedData")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SeedSimpleData()
        {
            try
            {
                _logger.LogInformation("Starting simple data seeding...");
                
                await _simpleSeedingService.SeedAllDataAsync(isProduction: true);
                
                _logger.LogInformation("Simple data seeding completed successfully!");
                
                return Ok(ApiResponse<object>.OkWithMessage("Simple seeding completed successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during simple data seeding");
                return BadRequest(ApiResponse<object>.Fail($"Seeding failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Trigger comprehensive data seeding with realistic maritime data
        /// </summary>
        /// <returns>Seeding result</returns>
        [HttpPost("comprehensive")]
        [RequirePermission("SeedData")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> SeedComprehensiveData([FromQuery] bool forceReseed = false)
        {
            try
            {
                _logger.LogInformation("Starting comprehensive data seeding...");
                
                await _comprehensiveDataSeedingService.SeedAllAsync(forceReseed);
                
                _logger.LogInformation("Comprehensive data seeding completed successfully!");
                
                return Ok(ApiResponse<object>.OkWithMessage("Comprehensive seeding completed successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during comprehensive data seeding");
                return BadRequest(ApiResponse<object>.Fail($"Comprehensive seeding failed: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get current database status with detailed statistics
        /// </summary>
        /// <returns>Database statistics</returns>
        [HttpGet("status")]
        [RequirePermission("SeedData")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetStatus()
        {
            var stats = new
            {
                Users = await _context.Users.CountAsync(),
                Roles = await _context.Roles.CountAsync(),
                Permissions = await _context.Permissions.CountAsync(),
                Ports = await _context.Ports.CountAsync(),
                Ships = await _context.Ships.CountAsync(),
                Containers = await _context.Containers.CountAsync(),
                Berths = await _context.Berths.CountAsync(),
                BerthAssignments = await _context.BerthAssignments.CountAsync(),
                ShipContainers = await _context.ShipContainers.CountAsync(),
                ContainerMovements = await _context.ContainerMovements.CountAsync(),
                Events = await _context.Events.CountAsync(),
                Analytics = await _context.Analytics.CountAsync(),
                DatabaseSize = await GetDatabaseSizeAsync()
            };
            
            return Ok(ApiResponse<object>.Ok(stats));
        }

        /// <summary>
        /// Clear all data from database (DANGEROUS - use with caution)
        /// </summary>
        /// <returns>Clear result</returns>
        [HttpDelete("clear")]
        [RequirePermission("SeedData")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ClearDatabase([FromQuery] string confirmToken)
        {
            // Safety check - require confirmation token
            if (confirmToken != "CONFIRM_CLEAR_DATABASE")
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid confirmation token. Use 'CONFIRM_CLEAR_DATABASE' to confirm."));
            }

            try
            {
                _logger.LogWarning("Starting database clear operation...");
                
                await _comprehensiveDataSeedingService.SeedAllAsync(forceReseed: true);
                
                _logger.LogInformation("Database cleared and reseeded successfully!");
                
                return Ok(ApiResponse<object>.OkWithMessage("Database cleared and reseeded successfully!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database clear operation");
                return BadRequest(ApiResponse<object>.Fail($"Clear operation failed: {ex.Message}"));
            }
        }

        private async Task<string> GetDatabaseSizeAsync()
        {
            try
            {
                // For PostgreSQL - get database size
                var sizeQuery = @"
                    SELECT pg_size_pretty(pg_database_size(current_database())) as size";
                
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = sizeQuery;
                var result = await command.ExecuteScalarAsync();
                
                return result?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unable to determine";
            }
        }
    }
}