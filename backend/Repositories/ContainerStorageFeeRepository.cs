using Backend.Data;
using Backend.Models;

namespace Backend.Repositories
{
    public class ContainerStorageFeeRepository : Repository<ContainerStorageFee>, IContainerStorageFeeRepository
    {
        public ContainerStorageFeeRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
