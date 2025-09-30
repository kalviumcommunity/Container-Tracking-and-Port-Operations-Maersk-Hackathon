using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository implementation for Berth operations
    /// </summary>
    public class BerthRepository : Repository<Berth>, IBerthRepository
    {
        public BerthRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets berths by port
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Berths at the specified port</returns>
        public async Task<IEnumerable<Berth>> GetByPortAsync(int portId)
        {
            return await _dbSet
                .Where(b => b.PortId == portId)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets berths by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Berths with the specified status</returns>
        public async Task<IEnumerable<Berth>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(b => b.Status == status)
                .ToListAsync();
        }
        
        /// <summary>
        /// Gets berth with all its assignments
        /// </summary>
        /// <param name="berthId">The ID of the berth</param>
        /// <returns>Berth with its assignments</returns>
        public async Task<Berth> GetWithAssignmentsAsync(int berthId)
        {
            return await _dbSet
                .Include(b => b.Port)
                .Include(b => b.BerthAssignments)
                .ThenInclude(ba => ba.Container)
                .FirstOrDefaultAsync(b => b.BerthId == berthId);
        }
        
        /// <summary>
        /// Overrides the base GetAllAsync to include navigation properties
        /// </summary>
        /// <returns>Berths with navigation properties</returns>
        public new async Task<IEnumerable<Berth>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.Port)
                .ToListAsync();
        }
    }
}