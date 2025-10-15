using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    public class ShipRouteRepository : Repository<ShipRoute>, IShipRouteRepository
    {
        public ShipRouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ShipRoute>> GetByShipIdAsync(int shipId)
        {
            return await _dbSet
                .Include(sr => sr.Ship)
                .Include(sr => sr.OriginPort)
                .Include(sr => sr.DestinationPort)
                .Where(sr => sr.ShipId == shipId)
                .ToListAsync();
        }

        public new async Task<IEnumerable<ShipRoute>> GetAllAsync()
        {
             return await _dbSet
                .Include(sr => sr.Ship)
                .Include(sr => sr.OriginPort)
                .Include(sr => sr.DestinationPort)
                .ToListAsync();
        }

        public new async Task<ShipRoute> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(sr => sr.Ship)
                .Include(sr => sr.OriginPort)
                .Include(sr => sr.DestinationPort)
                .FirstOrDefaultAsync(sr => sr.Id == (int)id);
        }
    }
}
