using Backend.Data;
using Backend.Models;

namespace Backend.Repositories
{
    public class BerthUsageChargeRepository : Repository<BerthUsageCharge>, IBerthUsageChargeRepository
    {
        public BerthUsageChargeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
