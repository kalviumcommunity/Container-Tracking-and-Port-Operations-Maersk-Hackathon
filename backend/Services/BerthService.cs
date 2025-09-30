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
    /// Service for Berth operations
    /// </summary>
    public class BerthService : IBerthService
    {
        private readonly IBerthRepository _berthRepository;

        public BerthService(IBerthRepository berthRepository)
        {
            _berthRepository = berthRepository;
        }

        /// <summary>
        /// Gets all berths
        /// </summary>
        /// <returns>All berths</returns>
        public async Task<IEnumerable<BerthDto>> GetAllAsync()
        {
            var berths = await _berthRepository.GetAllAsync();
            return berths.Select(MapToDto);
        }

        /// <summary>
        /// Gets a berth by its ID
        /// </summary>
        /// <param name="id">The ID of the berth</param>
        /// <returns>The berth or null if not found</returns>
        public async Task<BerthDto> GetByIdAsync(object id)
        {
            var berth = await _berthRepository.GetByIdAsync(id);
            return berth == null ? null : MapToDto(berth);
        }

        /// <summary>
        /// Gets a berth by ID with detailed information
        /// </summary>
        /// <param name="berthId">The ID of the berth</param>
        /// <returns>Berth with detailed information</returns>
        public async Task<BerthDetailDto> GetBerthDetailAsync(int berthId)
        {
            var berth = await _berthRepository.GetWithAssignmentsAsync(berthId);
            
            if (berth == null)
            {
                return null;
            }
            
            return new BerthDetailDto
            {
                BerthId = berth.BerthId,
                Name = berth.Name,
                Capacity = berth.Capacity,
                Status = berth.Status,
                PortId = berth.PortId,
                PortName = berth.Port?.Name,
                ActiveAssignmentCount = berth.BerthAssignments?.Count(ba => ba.ReleasedAt == null) ?? 0,
                BerthAssignments = berth.BerthAssignments?.Select(ba => new BerthAssignmentDto
                {
                    Id = ba.Id,
                    ContainerId = ba.ContainerId,
                    ContainerName = ba.Container?.Name,
                    BerthId = ba.BerthId,
                    BerthName = berth.Name,
                    AssignedAt = ba.AssignedAt,
                    ReleasedAt = ba.ReleasedAt
                }).ToList()
            };
        }

        /// <summary>
        /// Gets berths by port
        /// </summary>
        /// <param name="portId">The ID of the port</param>
        /// <returns>Berths at the specified port</returns>
        public async Task<IEnumerable<BerthDto>> GetByPortAsync(int portId)
        {
            var berths = await _berthRepository.GetByPortAsync(portId);
            return berths.Select(MapToDto);
        }

        /// <summary>
        /// Gets berths by status
        /// </summary>
        /// <param name="status">The status to filter by</param>
        /// <returns>Berths with the specified status</returns>
        public async Task<IEnumerable<BerthDto>> GetByStatusAsync(string status)
        {
            var berths = await _berthRepository.GetByStatusAsync(status);
            return berths.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<BerthDto>> GetAsync(Expression<Func<Berth, bool>> filter)
        {
            var berths = await _berthRepository.GetAsync(filter);
            return berths.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new berth
        /// </summary>
        /// <param name="createDto">Berth data</param>
        /// <returns>The created berth</returns>
        public async Task<BerthDto> CreateAsync(BerthCreateUpdateDto createDto)
        {
            var berth = new Berth
            {
                Name = createDto.Name,
                Capacity = createDto.Capacity,
                Status = createDto.Status,
                PortId = createDto.PortId
            };
            
            var createdBerth = await _berthRepository.CreateAsync(berth);
            return MapToDto(createdBerth);
        }

        /// <summary>
        /// Updates an existing berth
        /// </summary>
        /// <param name="id">The ID of the berth to update</param>
        /// <param name="updateDto">Updated berth data</param>
        /// <returns>The updated berth</returns>
        public async Task<BerthDto> UpdateAsync(object id, BerthCreateUpdateDto updateDto)
        {
            var existingBerth = await _berthRepository.GetByIdAsync(id);
            if (existingBerth == null)
            {
                throw new KeyNotFoundException($"Berth with ID {id} not found");
            }
            
            // Update fields
            existingBerth.Name = updateDto.Name;
            existingBerth.Capacity = updateDto.Capacity;
            existingBerth.Status = updateDto.Status;
            existingBerth.PortId = updateDto.PortId;
            
            var updatedBerth = await _berthRepository.UpdateAsync(existingBerth);
            return MapToDto(updatedBerth);
        }

        /// <summary>
        /// Deletes a berth
        /// </summary>
        /// <param name="id">The ID of the berth to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _berthRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a Berth entity to a BerthDto
        /// </summary>
        /// <param name="berth">The berth entity</param>
        /// <returns>A berth DTO</returns>
        private static BerthDto MapToDto(Berth berth)
        {
            return new BerthDto
            {
                BerthId = berth.BerthId,
                Name = berth.Name,
                Capacity = berth.Capacity,
                Status = berth.Status,
                PortId = berth.PortId,
                PortName = berth.Port?.Name,
                ActiveAssignmentCount = berth.BerthAssignments?.Count(ba => ba.ReleasedAt == null) ?? 0
            };
        }
    }
}