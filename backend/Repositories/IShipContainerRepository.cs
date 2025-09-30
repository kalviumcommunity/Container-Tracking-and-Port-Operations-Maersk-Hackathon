using Backend.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for ShipContainer operations
    /// </summary>
    public interface IShipContainerRepository : IRepository<ShipContainer>
    {
        /// <summary>
        /// Gets operations by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Operations for the specified container</returns>
        Task<IEnumerable<ShipContainer>> GetByContainerIdAsync(string containerId);
        
        /// <summary>
        /// Gets operations by ship ID
        /// </summary>
        /// <param name="shipId">The ship ID</param>
        /// <returns>Operations for the specified ship</returns>
        Task<IEnumerable<ShipContainer>> GetByShipIdAsync(int shipId);
        
        /// <summary>
        /// Gets operations within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Operations within the specified date range</returns>
        Task<IEnumerable<ShipContainer>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}