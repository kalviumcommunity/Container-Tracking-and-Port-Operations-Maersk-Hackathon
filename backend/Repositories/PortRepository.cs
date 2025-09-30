using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for Port operations
    /// </summary>
    public class PortRepository : Repository<Port>, IPortRepository
    {
        public PortRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets ports by location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Ports at the specified location</returns>
        public async Task<IEnumerable<Port>> GetByLocationAsync(string location)
        {
            return await _dbSet
                .Where(p => p.Location.Contains(location))
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets port with all related berths
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Port with its berths</returns>
        public async Task<Port> GetWithBerthsAsync(int portId)
        {
            return await _dbSet
                .Include(p => p.Berths)
                .FirstOrDefaultAsync(p => p.PortId == portId);
        }
        
        /// <summary>
        /// Overrides the base GetAllAsync to include navigation properties
        /// </summary>
        /// <returns>Ports with navigation properties</returns>
        public new async Task<IEnumerable<Port>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.Berths)
                .ToListAsync();
        }
    }
}