using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public interface IShipRouteRepository : IRepository<ShipRoute>
    {
        Task<IEnumerable<ShipRoute>> GetByShipIdAsync(int shipId);
    }
}
