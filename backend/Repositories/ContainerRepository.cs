using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for Container operations
    /// </summary>
    public class ContainerRepository : Repository<Container>, IContainerRepository
    {
        public ContainerRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets containers by their current location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Containers at the specified location</returns>
        public async Task<IEnumerable<Container>> GetByLocationAsync(string location)
        {
            return await _dbSet
                .Where(c => c.CurrentLocation == location)
                .ToListAsync();
        }

        /// <summary>
        /// Gets containers by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Containers with the specified status</returns>
        public async Task<IEnumerable<Container>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(c => c.Status == status)
                .ToListAsync();
        }

        /// <summary>
        /// Gets containers by the ship they're on
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Containers on the specified ship</returns>
        public async Task<IEnumerable<Container>> GetByShipIdAsync(int shipId)
        {
            return await _dbSet
                .Where(c => c.ShipId == shipId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Overrides the base GetByIdAsync to include navigation properties
        /// </summary>
        /// <param name="id">The ID of the container</param>
        /// <returns>Container with navigation properties</returns>
        public async Task<Container> GetByIdWithDetailsAsync(string id)
        {
            return await _dbSet
                .Include(c => c.Ship)
                .Include(c => c.BerthAssignments)
                .ThenInclude(ba => ba.Berth)
                .Include(c => c.ShipContainers)
                .ThenInclude(sc => sc.Ship)
                .FirstOrDefaultAsync(c => c.ContainerId == id);
        }
    }
}