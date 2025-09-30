using Backend.DTOs;
using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service interface for Container operations
    /// </summary>
    public interface IContainerService : IService<Container, ContainerDto, ContainerCreateUpdateDto>
    {
        /// <summary>
        /// Gets a container by ID with detailed information
        /// </summary>
        /// <param name="containerId">The ID of the container</param>
        /// <returns>Container with detailed information</returns>
        Task<ContainerDetailDto> GetContainerDetailAsync(string containerId);
        
        /// <summary>
        /// Gets containers by their current location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Containers at the specified location</returns>
        Task<IEnumerable<ContainerDto>> GetByLocationAsync(string location);
        
        /// <summary>
        /// Gets containers by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Containers with the specified status</returns>
        Task<IEnumerable<ContainerDto>> GetByStatusAsync(string status);
        
        /// <summary>
        /// Gets containers by the ship they're on
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Containers on the specified ship</returns>
        Task<IEnumerable<ContainerDto>> GetByShipIdAsync(int shipId);
    }
}