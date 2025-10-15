using Backend.DTOs;
using Backend.Services;
using Backend.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/container-storage-fees")]
    [Authorize]
    public class ContainerStorageFeesController : ControllerBase
    {
        private readonly IContainerStorageFeeService _containerStorageFeeService;

        public ContainerStorageFeesController(IContainerStorageFeeService containerStorageFeeService)
        {
            _containerStorageFeeService = containerStorageFeeService;
        }

        [HttpGet]
        [RequirePermission("ViewReports")]
        public async Task<IActionResult> GetAll()
        {
            var fees = await _containerStorageFeeService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(fees));
        }

        [HttpGet("{id}")]
        [RequirePermission("ViewReports")]
        public async Task<IActionResult> GetById(int id)
        {
            var fee = await _containerStorageFeeService.GetByIdAsync(id);
            if (fee == null)
            {
                return NotFound(ApiResponse<object>.Fail("Container storage fee not found"));
            }
            return Ok(ApiResponse<object>.Ok(fee));
        }

        [HttpGet("container/{containerId}")]
        [RequirePermission("ViewReports")]
        public async Task<IActionResult> GetByContainerId(string containerId)
        {
            var fees = await _containerStorageFeeService.GetAsync(f => f.ContainerId == containerId);
            return Ok(ApiResponse<object>.Ok(fees));
        }

        [HttpPost]
        [RequirePermission("GenerateReports")]
        public async Task<IActionResult> Create([FromBody] ContainerStorageFeeCreateUpdateDto createDto)
        {
            var fee = await _containerStorageFeeService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = fee.Id }, ApiResponse<object>.Ok(fee));
        }

        [HttpPut("{id}")]
        [RequirePermission("GenerateReports")]
        public async Task<IActionResult> Update(int id, [FromBody] ContainerStorageFeeCreateUpdateDto updateDto)
        {
            try
            {
                var fee = await _containerStorageFeeService.UpdateAsync(id, updateDto);
                return Ok(ApiResponse<object>.Ok(fee));
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("Container storage fee not found"));
            }
        }

        [HttpDelete("{id}")]
        [RequirePermission("GenerateReports")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _containerStorageFeeService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail("Container storage fee not found"));
            }
            return Ok(ApiResponse<object>.OkWithMessage("Container storage fee deleted successfully"));
        }
    }
}
