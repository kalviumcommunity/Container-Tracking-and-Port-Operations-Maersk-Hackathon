using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for ShipContainer operations
    /// </summary>
    public class ShipContainerRepository : Repository<ShipContainer>, IShipContainerRepository
    {
        public ShipContainerRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets operations by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Operations for the specified container</returns>
        public async Task<IEnumerable<ShipContainer>> GetByContainerIdAsync(string containerId)
        {
            return await _dbSet
                .Include(sc => sc.Ship)
                .Include(sc => sc.Container)
                .Where(sc => sc.ContainerId == containerId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets operations by ship ID
        /// </summary>
        /// <param name="shipId">The ship ID</param>
        /// <returns>Operations for the specified ship</returns>
        public async Task<IEnumerable<ShipContainer>> GetByShipIdAsync(int shipId)
        {
            return await _dbSet
                .Include(sc => sc.Ship)
                .Include(sc => sc.Container)
                .Where(sc => sc.ShipId == shipId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets operations within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Operations within the specified date range</returns>
        public async Task<IEnumerable<ShipContainer>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(sc => sc.Ship)
                .Include(sc => sc.Container)
                .Where(sc => sc.LoadedAt >= startDate && sc.LoadedAt <= endDate)
                .ToListAsync();
        }
        
        /// <summary>
        /// Overrides the base GetAllAsync to include navigation properties
        /// </summary>
        /// <returns>ShipContainers with navigation properties</returns>
        public new async Task<IEnumerable<ShipContainer>> GetAllAsync()
        {
            return await _dbSet
                .Include(sc => sc.Ship)
                .Include(sc => sc.Container)
                .ToListAsync();
        }
    }
}