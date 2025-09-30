using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for Ship operations
    /// </summary>
    public class ShipRepository : Repository<Ship>, IShipRepository
    {
        public ShipRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets ships by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Ships with the specified status</returns>
        public async Task<IEnumerable<Ship>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(s => s.Status == status)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets ship with all related containers
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Ship with its containers</returns>
        public async Task<Ship> GetWithContainersAsync(int shipId)
        {
            return await _dbSet
                .Include(s => s.Containers)
                .Include(s => s.ShipContainers)
                .ThenInclude(sc => sc.Container)
                .FirstOrDefaultAsync(s => s.ShipId == shipId);
        }
        
        /// <summary>
        /// Overrides the base GetAllAsync to include navigation properties
        /// </summary>
        /// <returns>Ships with navigation properties</returns>
        public new async Task<IEnumerable<Ship>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Containers)
                .ToListAsync();
        }
    }
}