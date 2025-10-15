using Backend.DTOs;
using Backend.Models;

namespace Backend.Services
{
    public interface IBerthUsageChargeService : IService<BerthUsageCharge, BerthUsageChargeDto, BerthUsageChargeCreateUpdateDto>
    {
    }
}
