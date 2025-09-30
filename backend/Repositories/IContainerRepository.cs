using Backend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for Container operations
    /// </summary>
    public interface IContainerRepository : IRepository<Container>
    {
        /// <summary>
        /// Gets containers by their current location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Containers at the specified location</returns>
        Task<IEnumerable<Container>> GetByLocationAsync(string location);
        
        /// <summary>
        /// Gets containers by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Containers with the specified status</returns>
        Task<IEnumerable<Container>> GetByStatusAsync(string status);
        
        /// <summary>
        /// Gets containers by the ship they're on
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Containers on the specified ship</returns>
        Task<IEnumerable<Container>> GetByShipIdAsync(int shipId);
    }
}