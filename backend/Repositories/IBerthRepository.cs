using Backend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for Berth operations
    /// </summary>
    public interface IBerthRepository : IRepository<Berth>
    {
        /// <summary>
        /// Gets berths by port
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Berths at the specified port</returns>
        Task<IEnumerable<Berth>> GetByPortAsync(int portId);
        
        /// <summary>
        /// Gets berths by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Berths with the specified status</returns>
        Task<IEnumerable<Berth>> GetByStatusAsync(string status);
        
        /// <summary>
        /// Gets berth with all its assignments
        /// </summary>
        /// <param name="berthId">The ID of the berth</param>
        /// <returns>Berth with its assignments</returns>
        Task<Berth> GetWithAssignmentsAsync(int berthId);
    }
}