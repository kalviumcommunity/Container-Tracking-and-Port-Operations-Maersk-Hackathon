using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/berth-usage-charges")]
    [Authorize]
    public class BerthUsageChargesController : ControllerBase
    {
        private readonly IBerthUsageChargeService _berthUsageChargeService;

        public BerthUsageChargesController(IBerthUsageChargeService berthUsageChargeService)
        {
            _berthUsageChargeService = berthUsageChargeService;
        }

        [HttpGet]
        [RequirePermission("ViewReports")]
        public async Task<IActionResult> GetAll()
        {
            var charges = await _berthUsageChargeService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(charges));
        }

        [HttpGet("{id}")]
        [RequirePermission("ViewReports")]
        public async Task<IActionResult> GetById(int id)
        {
            var charge = await _berthUsageChargeService.GetByIdAsync(id);
            if (charge == null)
            {
                return NotFound(ApiResponse<object>.Fail("Berth usage charge not found"));
            }
            return Ok(ApiResponse<object>.Ok(charge));
        }

        [HttpPost]
        [RequirePermission("GenerateReports")]
        public async Task<IActionResult> Create([FromBody] BerthUsageChargeCreateUpdateDto createDto)
        {
            var charge = await _berthUsageChargeService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = charge.Id }, ApiResponse<object>.Ok(charge));
        }

        [HttpPut("{id}")]
        [RequirePermission("GenerateReports")]
        public async Task<IActionResult> Update(int id, [FromBody] BerthUsageChargeCreateUpdateDto updateDto)
        {
            try
            {
                var charge = await _berthUsageChargeService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<object>.Ok(charge));
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Berth usage charge not found"));
            }
        }

        [HttpDelete("{id}")]
        [RequirePermission("GenerateReports")]
        public async Task<IActionResult> Delete(int id)
        { 
            var result = await _berthUsageChargeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail("Berth usage charge not found"));
            }
            return Ok(ApiResponse<object>.OkWithMessage("Berth usage charge deleted successfully"));
        }
    }
}
