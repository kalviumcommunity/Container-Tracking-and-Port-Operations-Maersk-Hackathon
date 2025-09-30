using Backend.DTOs;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service interface for Port operations
    /// </summary>
    public interface IPortService : IService<Port, PortDto, PortCreateUpdateDto>
    {
        /// <summary>
        /// Gets a port by ID with detailed information
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Port with detailed information</returns>
        Task<PortDetailDto> GetPortDetailAsync(int portId);
        
        /// <summary>
        /// Gets ports by location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Ports at the specified location</returns>
        Task<IEnumerable<PortDto>> GetByLocationAsync(string location);
    }
}