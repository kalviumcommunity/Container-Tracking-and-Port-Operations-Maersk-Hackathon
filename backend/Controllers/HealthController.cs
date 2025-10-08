using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public HealthController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Liveness probe
        [HttpGet("live")]
        [AllowAnonymous]
        public IActionResult Live() => Ok(new { status = "ok" });

        // Readiness probe with a lightweight DB check
        [HttpGet("ready")]
        [AllowAnonymous]
        public async Task<IActionResult> Ready()
        {
            try
            {
                await _db.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok(new { status = "ready" });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new { status = "degraded", error = ex.Message });
            }
        }
    }
}
