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
    /// Service for ShipContainer operations
    /// </summary>
    public class ShipContainerService : IShipContainerService
    {
        private readonly IShipContainerRepository _shipContainerRepository;

        public ShipContainerService(IShipContainerRepository shipContainerRepository)
        {
            _shipContainerRepository = shipContainerRepository;
        }

        /// <summary>
        /// Gets all ship container operations
        /// </summary>
        /// <returns>All ship container operations</returns>
        public async Task<IEnumerable<ShipContainerDto>> GetAllAsync()
        {
            var operations = await _shipContainerRepository.GetAllAsync();
            return operations.Select(MapToDto);
        }

        /// <summary>
        /// Gets a ship container operation by its ID
        /// </summary>
        /// <param name="id">The ID of the ship container operation</param>
        /// <returns>The ship container operation or null if not found</returns>
        public async Task<ShipContainerDto> GetByIdAsync(object id)
        {
            var operation = await _shipContainerRepository.GetByIdAsync(id);
            return operation == null ? null : MapToDto(operation);
        }

        /// <summary>
        /// Gets operations by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Operations for the specified container</returns>
        public async Task<IEnumerable<ShipContainerDto>> GetByContainerIdAsync(string containerId)
        {
            var operations = await _shipContainerRepository.GetByContainerIdAsync(containerId);
            return operations.Select(MapToDto);
        }

        /// <summary>
        /// Gets operations by ship ID
        /// </summary>
        /// <param name="shipId">The ship ID</param>
        /// <returns>Operations for the specified ship</returns>
        public async Task<IEnumerable<ShipContainerDto>> GetByShipIdAsync(int shipId)
        {
            var operations = await _shipContainerRepository.GetByShipIdAsync(shipId);
            return operations.Select(MapToDto);
        }

        /// <summary>
        /// Gets operations within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Operations within the specified date range</returns>
        public async Task<IEnumerable<ShipContainerDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var operations = await _shipContainerRepository.GetByDateRangeAsync(startDate, endDate);
            return operations.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<ShipContainerDto>> GetAsync(Expression<Func<ShipContainer, bool>> filter)
        {
            var operations = await _shipContainerRepository.GetAsync(filter);
            return operations.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new ship container operation
        /// </summary>
        /// <param name="createDto">Ship container operation data</param>
        /// <returns>The created ship container operation</returns>
        public async Task<ShipContainerDto> CreateAsync(ShipContainerCreateUpdateDto createDto)
        {
            var operation = new ShipContainer
            {
                ShipId = createDto.ShipId,
                ContainerId = createDto.ContainerId,
                LoadedAt = createDto.LoadedAt ?? DateTime.UtcNow
            };
            
            var createdOperation = await _shipContainerRepository.CreateAsync(operation);
            return MapToDto(createdOperation);
        }

        /// <summary>
        /// Updates an existing ship container operation
        /// </summary>
        /// <param name="id">The ID of the ship container operation to update</param>
        /// <param name="updateDto">Updated ship container operation data</param>
        /// <returns>The updated ship container operation</returns>
        public async Task<ShipContainerDto> UpdateAsync(object id, ShipContainerCreateUpdateDto updateDto)
        {
            var existingOperation = await _shipContainerRepository.GetByIdAsync(id);
            if (existingOperation == null)
            {
                throw new KeyNotFoundException($"Ship container operation with ID {id} not found");
            }
            
            // Update fields
            existingOperation.ShipId = updateDto.ShipId;
            existingOperation.ContainerId = updateDto.ContainerId;
            if (updateDto.LoadedAt.HasValue)
                existingOperation.LoadedAt = updateDto.LoadedAt.Value;
            
            var updatedOperation = await _shipContainerRepository.UpdateAsync(existingOperation);
            return MapToDto(updatedOperation);
        }

        /// <summary>
        /// Loads a container onto a ship
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <param name="containerId">The ID of the container</param>
        /// <returns>The created ship container operation</returns>
        public async Task<ShipContainerDto> LoadContainerAsync(int shipId, string containerId)
        {
            var operation = new ShipContainer
            {
                ShipId = shipId,
                ContainerId = containerId,
                LoadedAt = DateTime.UtcNow
            };
            
            var createdOperation = await _shipContainerRepository.CreateAsync(operation);
            return MapToDto(createdOperation);
        }

        /// <summary>
        /// Deletes a ship container operation
        /// </summary>
        /// <param name="id">The ID of the ship container operation to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _shipContainerRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a ShipContainer entity to a ShipContainerDto
        /// </summary>
        /// <param name="operation">The ship container operation entity</param>
        /// <returns>A ship container operation DTO</returns>
        private static ShipContainerDto MapToDto(ShipContainer operation)
        {
            return new ShipContainerDto
            {
                Id = operation.Id,
                ShipId = operation.ShipId,
                ShipName = operation.Ship?.Name,
                ContainerId = operation.ContainerId,
                ContainerName = operation.Container?.CargoType,
                LoadedAt = operation.LoadedAt
            };
        }
    }
}