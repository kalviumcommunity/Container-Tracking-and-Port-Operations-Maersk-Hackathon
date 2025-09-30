using Backend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for Port operations
    /// </summary>
    public interface IPortRepository : IRepository<Port>
    {
        /// <summary>
        /// Gets ports by location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Ports at the specified location</returns>
        Task<IEnumerable<Port>> GetByLocationAsync(string location);
        
        /// <summary>
        /// Gets port with all related berths
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Port with its berths</returns>
        Task<Port> GetWithBerthsAsync(int portId);
    }
}