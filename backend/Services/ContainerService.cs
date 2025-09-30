using Backend.DTOs;
using Backend.Models;
using Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service for Container operations
    /// </summary>
    public class ContainerService : IContainerService
    {
        private readonly IContainerRepository _containerRepository;

        public ContainerService(IContainerRepository containerRepository)
        {
            _containerRepository = containerRepository;
        }

        /// <summary>
        /// Gets all containers
        /// </summary>
        /// <returns>All containers</returns>
        public async Task<IEnumerable<ContainerDto>> GetAllAsync()
        {
            var containers = await _containerRepository.GetAllAsync();
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets a container by its ID
        /// </summary>
        /// <param name="id">The ID of the container</param>
        /// <returns>The container or null if not found</returns>
        public async Task<ContainerDto> GetByIdAsync(object id)
        {
            var container = await _containerRepository.GetByIdAsync(id);
            return container == null ? null : MapToDto(container);
        }

        /// <summary>
        /// Gets a container by ID with detailed information
        /// </summary>
        /// <param name="containerId">The ID of the container</param>
        /// <returns>Container with detailed information</returns>
        public async Task<ContainerDetailDto> GetContainerDetailAsync(string containerId)
        {
            // Using the repository method to get container with details
            // This requires adding a method to the ContainerRepository
            var container = await ((ContainerRepository)_containerRepository).GetByIdWithDetailsAsync(containerId);
            
            if (container == null)
            {
                return null;
            }
            
            return new ContainerDetailDto
            {
                ContainerId = container.ContainerId,
                Name = container.Name,
                Type = container.Type,
                Status = container.Status,
                CurrentLocation = container.CurrentLocation,
                CreatedAt = container.CreatedAt,
                UpdatedAt = container.UpdatedAt,
                ShipId = container.ShipId,
                ShipName = container.Ship?.Name,
                BerthAssignments = container.BerthAssignments?.Select(ba => new BerthAssignmentDto
                {
                    Id = ba.Id,
                    ContainerId = ba.ContainerId,
                    ContainerName = container.Name,
                    BerthId = ba.BerthId,
                    BerthName = ba.Berth?.Name,
                    AssignedAt = ba.AssignedAt,
                    ReleasedAt = ba.ReleasedAt
                }).ToList(),
                ShipContainers = container.ShipContainers?.Select(sc => new ShipContainerDto
                {
                    Id = sc.Id,
                    ShipId = sc.ShipId,
                    ShipName = sc.Ship?.Name,
                    ContainerId = sc.ContainerId,
                    ContainerName = container.Name,
                    LoadedAt = sc.LoadedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Gets containers by their current location
        /// </summary>
        /// <param name="location">The location to filter by</param>
        /// <returns>Containers at the specified location</returns>
        public async Task<IEnumerable<ContainerDto>> GetByLocationAsync(string location)
        {
            var containers = await _containerRepository.GetByLocationAsync(location);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets containers by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Containers with the specified status</returns>
        public async Task<IEnumerable<ContainerDto>> GetByStatusAsync(string status)
        {
            var containers = await _containerRepository.GetByStatusAsync(status);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets containers by the ship they're on
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Containers on the specified ship</returns>
        public async Task<IEnumerable<ContainerDto>> GetByShipIdAsync(int shipId)
        {
            var containers = await _containerRepository.GetByShipIdAsync(shipId);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<ContainerDto>> GetAsync(Expression<Func<Container, bool>> filter)
        {
            var containers = await _containerRepository.GetAsync(filter);
            return containers.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new container
        /// </summary>
        /// <param name="createDto">Container data</param>
        /// <returns>The created container</returns>
        public async Task<ContainerDto> CreateAsync(ContainerCreateUpdateDto createDto)
        {
            var container = new Container
            {
                ContainerId = createDto.ContainerId,
                Name = createDto.Name,
                Type = createDto.Type,
                Status = createDto.Status,
                CurrentLocation = createDto.CurrentLocation,
                ShipId = createDto.ShipId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            var createdContainer = await _containerRepository.CreateAsync(container);
            return MapToDto(createdContainer);
        }

        /// <summary>
        /// Updates an existing container
        /// </summary>
        /// <param name="id">The ID of the container to update</param>
        /// <param name="updateDto">Updated container data</param>
        /// <returns>The updated container</returns>
        public async Task<ContainerDto> UpdateAsync(object id, ContainerCreateUpdateDto updateDto)
        {
            var existingContainer = await _containerRepository.GetByIdAsync(id);
            if (existingContainer == null)
            {
                throw new KeyNotFoundException($"Container with ID {id} not found");
            }
            
            // Update fields
            existingContainer.Name = updateDto.Name;
            existingContainer.Type = updateDto.Type;
            existingContainer.Status = updateDto.Status;
            existingContainer.CurrentLocation = updateDto.CurrentLocation;
            existingContainer.ShipId = updateDto.ShipId;
            existingContainer.UpdatedAt = DateTime.UtcNow;
            
            var updatedContainer = await _containerRepository.UpdateAsync(existingContainer);
            return MapToDto(updatedContainer);
        }

        /// <summary>
        /// Deletes a container
        /// </summary>
        /// <param name="id">The ID of the container to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _containerRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a Container entity to a ContainerDto
        /// </summary>
        /// <param name="container">The container entity</param>
        /// <returns>A container DTO</returns>
        private static ContainerDto MapToDto(Container container)
        {
            return new ContainerDto
            {
                ContainerId = container.ContainerId,
                Name = container.Name,
                Type = container.Type,
                Status = container.Status,
                CurrentLocation = container.CurrentLocation,
                CreatedAt = container.CreatedAt,
                UpdatedAt = container.UpdatedAt,
                ShipId = container.ShipId,
                ShipName = container.Ship?.Name
            };
        }
    }
}