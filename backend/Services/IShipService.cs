using Backend.DTOs;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service interface for Ship operations
    /// </summary>
    public interface IShipService : IService<Ship, ShipDto, ShipCreateUpdateDto>
    {
        /// <summary>
        /// Gets a ship by ID with detailed information
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Ship with detailed information</returns>
        Task<ShipDetailDto> GetShipDetailAsync(int shipId);
        
        /// <summary>
        /// Gets ships by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Ships with the specified status</returns>
        Task<IEnumerable<ShipDto>> GetByStatusAsync(string status);
    }
}