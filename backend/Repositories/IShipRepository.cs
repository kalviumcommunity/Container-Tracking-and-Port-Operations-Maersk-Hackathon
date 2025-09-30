using Backend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for Ship operations
    /// </summary>
    public interface IShipRepository : IRepository<Ship>
    {
        /// <summary>
        /// Gets ships by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Ships with the specified status</returns>
        Task<IEnumerable<Ship>> GetByStatusAsync(string status);
        
        /// <summary>
        /// Gets ship with all related containers
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Ship with its containers</returns>
        Task<Ship> GetWithContainersAsync(int shipId);
    }
}