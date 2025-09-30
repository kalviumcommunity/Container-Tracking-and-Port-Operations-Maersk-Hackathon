using Backend.DTOs;
using Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service interface for ShipContainer operations
    /// </summary>
    public interface IShipContainerService : IService<ShipContainer, ShipContainerDto, ShipContainerCreateUpdateDto>
    {
        /// <summary>
        /// Gets operations by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Operations for the specified container</returns>
        Task<IEnumerable<ShipContainerDto>> GetByContainerIdAsync(string containerId);
        
        /// <summary>
        /// Gets operations by ship ID
        /// </summary>
        /// <param name="shipId">The ship ID</param>
        /// <returns>Operations for the specified ship</returns>
        Task<IEnumerable<ShipContainerDto>> GetByShipIdAsync(int shipId);
        
        /// <summary>
        /// Gets operations within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Operations within the specified date range</returns>
        Task<IEnumerable<ShipContainerDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Loads a container onto a ship
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <param name="containerId">The ID of the container</param>
        /// <returns>The created ship container operation</returns>
        Task<ShipContainerDto> LoadContainerAsync(int shipId, string containerId);
    }
}