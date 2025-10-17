using Backend.DTOs;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service interface for Berth operations
    /// </summary>
    public interface IBerthService : IService<Berth, BerthDto, BerthCreateUpdateDto>
    {
        /// <summary>
        /// Gets a berth by ID with detailed information
        /// </summary>
        /// <param name="berthId">The ID of the berth</param>
        /// <returns>Berth with detailed information</returns>
        Task<BerthDetailDto> GetBerthDetailAsync(int berthId);
        
        /// <summary>
        /// Gets berths by port
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Berths at the specified port</returns>
        Task<IEnumerable<BerthDto>> GetByPortAsync(int portId);
        
        /// <summary>
        /// Gets berths by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Berths with the specified status</returns>
        Task<IEnumerable<BerthDto>> GetByStatusAsync(string status);
    }
}