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
    /// Service for Ship operations
    /// </summary>
    public class ShipService : IShipService
    {
        private readonly IShipRepository _shipRepository;

        public ShipService(IShipRepository shipRepository)
        {
            _shipRepository = shipRepository;
        }

        /// <summary>
        /// Gets all ships
        /// </summary>
        /// <returns>All ships</returns>
        public async Task<IEnumerable<ShipDto>> GetAllAsync()
        {
            var ships = await _shipRepository.GetAllAsync();
            return ships.Select(MapToDto);
        }

        /// <summary>
        /// Gets a ship by its ID
        /// </summary>
        /// <param name="id">The ID of the ship</param>
        /// <returns>The ship or null if not found</returns>
        public async Task<ShipDto> GetByIdAsync(object id)
        {
            var ship = await _shipRepository.GetByIdAsync(id);
            return ship == null ? null : MapToDto(ship);
        }

        /// <summary>
        /// Gets a ship by ID with detailed information
        /// </summary>
        /// <param name="shipId">The ID of the ship</param>
        /// <returns>Ship with detailed information</returns>
        public async Task<ShipDetailDto> GetShipDetailAsync(int shipId)
        {
            var ship = await _shipRepository.GetWithContainersAsync(shipId);
            
            if (ship == null)
            {
                return null;
            }
            
            return new ShipDetailDto
            {
                ShipId = ship.ShipId,
                Name = ship.Name,
                Status = ship.Status,
                ContainerCount = ship.Containers?.Count ?? 0,
                Containers = ship.Containers?.Select(c => new ContainerDto
                {
                    ContainerId = c.ContainerId,
                    CargoType = c.CargoType,
                    Type = c.Type,
                    Status = c.Status,
                    CurrentLocation = c.CurrentLocation,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ShipId = c.ShipId,
                    ShipName = ship.Name
                }).ToList(),
                ShipContainers = ship.ShipContainers?.Select(sc => new ShipContainerDto
                {
                    Id = sc.Id,
                    ShipId = sc.ShipId,
                    ShipName = ship.Name,
                    ContainerId = sc.ContainerId,
                    ContainerName = sc.Container?.CargoType,
                    LoadedAt = sc.LoadedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Gets ships by their status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Ships with the specified status</returns>
        public async Task<IEnumerable<ShipDto>> GetByStatusAsync(string status)
        {
            var ships = await _shipRepository.GetByStatusAsync(status);
            return ships.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<ShipDto>> GetAsync(Expression<Func<Ship, bool>> filter)
        {
            var ships = await _shipRepository.GetAsync(filter);
            return ships.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new ship
        /// </summary>
        /// <param name="createDto">Ship data</param>
        /// <returns>The created ship</returns>
        public async Task<ShipDto> CreateAsync(ShipCreateUpdateDto createDto)
        {
            var ship = new Ship
            {
                Name = createDto.Name,
                Status = createDto.Status
            };
            
            var createdShip = await _shipRepository.CreateAsync(ship);
            return MapToDto(createdShip);
        }

        /// <summary>
        /// Updates an existing ship
        /// </summary>
        /// <param name="id">The ID of the ship to update</param>
        /// <param name="updateDto">Updated ship data</param>
        /// <returns>The updated ship</returns>
        public async Task<ShipDto> UpdateAsync(object id, ShipCreateUpdateDto updateDto)
        {
            var existingShip = await _shipRepository.GetByIdAsync(id);
            if (existingShip == null)
            {
                throw new KeyNotFoundException($"Ship with ID {id} not found");
            }
            
            // Update fields
            existingShip.Name = updateDto.Name;
            existingShip.Status = updateDto.Status;
            
            var updatedShip = await _shipRepository.UpdateAsync(existingShip);
            return MapToDto(updatedShip);
        }

        /// <summary>
        /// Deletes a ship
        /// </summary>
        /// <param name="id">The ID of the ship to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _shipRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a Ship entity to a ShipDto
        /// </summary>
        /// <param name="ship">The ship entity</param>
        /// <returns>A ship DTO</returns>
        private static ShipDto MapToDto(Ship ship)
        {
            return new ShipDto
            {
                ShipId = ship.ShipId,
                Name = ship.Name,
                Status = ship.Status,
                ContainerCount = ship.Containers?.Count ?? 0
            };
        }
    }
}