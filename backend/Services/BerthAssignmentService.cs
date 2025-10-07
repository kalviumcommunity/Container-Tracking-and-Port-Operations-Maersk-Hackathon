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
    /// Service for BerthAssignment operations
    /// </summary>
    public class BerthAssignmentService : IBerthAssignmentService
    {
        private readonly IBerthAssignmentRepository _berthAssignmentRepository;

        public BerthAssignmentService(IBerthAssignmentRepository berthAssignmentRepository)
        {
            _berthAssignmentRepository = berthAssignmentRepository;
        }

        /// <summary>
        /// Gets all berth assignments
        /// </summary>
        /// <returns>All berth assignments</returns>
        public async Task<IEnumerable<BerthAssignmentDto>> GetAllAsync()
        {
            var assignments = await _berthAssignmentRepository.GetAllAsync();
            return assignments.Select(MapToDto);
        }

        /// <summary>
        /// Gets a berth assignment by its ID
        /// </summary>
        /// <param name="id">The ID of the berth assignment</param>
        /// <returns>The berth assignment or null if not found</returns>
        public async Task<BerthAssignmentDto> GetByIdAsync(object id)
        {
            var assignment = await _berthAssignmentRepository.GetByIdAsync(id);
            return assignment == null ? null : MapToDto(assignment);
        }

        /// <summary>
        /// Gets assignments by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Assignments for the specified container</returns>
        public async Task<IEnumerable<BerthAssignmentDto>> GetByContainerIdAsync(string containerId)
        {
            var assignments = await _berthAssignmentRepository.GetByContainerIdAsync(containerId);
            return assignments.Select(MapToDto);
        }

        /// <summary>
        /// Gets assignments by berth ID
        /// </summary>
        /// <param name="berthId">The berth ID</param>
        /// <returns>Assignments for the specified berth</returns>
        public async Task<IEnumerable<BerthAssignmentDto>> GetByBerthIdAsync(int berthId)
        {
            var assignments = await _berthAssignmentRepository.GetByBerthIdAsync(berthId);
            return assignments.Select(MapToDto);
        }

        /// <summary>
        /// Gets active assignments (where ReleasedAt is null)
        /// </summary>
        /// <returns>All active assignments</returns>
        public async Task<IEnumerable<BerthAssignmentDto>> GetActiveAssignmentsAsync()
        {
            var assignments = await _berthAssignmentRepository.GetActiveAssignmentsAsync();
            return assignments.Select(MapToDto);
        }

        /// <summary>
        /// Gets assignments within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Assignments within the specified date range</returns>
        public async Task<IEnumerable<BerthAssignmentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var assignments = await _berthAssignmentRepository.GetByDateRangeAsync(startDate, endDate);
            return assignments.Select(MapToDto);
        }

        /// <summary>
        /// Gets entities based on a filter expression
        /// </summary>
        /// <param name="filter">Expression to filter entities</param>
        /// <returns>Filtered entities</returns>
        public async Task<IEnumerable<BerthAssignmentDto>> GetAsync(Expression<Func<BerthAssignment, bool>> filter)
        {
            var assignments = await _berthAssignmentRepository.GetAsync(filter);
            return assignments.Select(MapToDto);
        }

        /// <summary>
        /// Creates a new berth assignment
        /// </summary>
        /// <param name="createDto">Berth assignment data</param>
        /// <returns>The created berth assignment</returns>
        public async Task<BerthAssignmentDto> CreateAsync(BerthAssignmentCreateUpdateDto createDto)
        {
            var assignment = new BerthAssignment
            {
                ContainerId = createDto.ContainerId,
                BerthId = createDto.BerthId,
                AssignedAt = createDto.AssignedAt ?? DateTime.UtcNow,
                ReleasedAt = createDto.ReleasedAt
            };
            
            var createdAssignment = await _berthAssignmentRepository.CreateAsync(assignment);
            return MapToDto(createdAssignment);
        }

        /// <summary>
        /// Updates an existing berth assignment
        /// </summary>
        /// <param name="id">The ID of the berth assignment to update</param>
        /// <param name="updateDto">Updated berth assignment data</param>
        /// <returns>The updated berth assignment</returns>
        public async Task<BerthAssignmentDto> UpdateAsync(object id, BerthAssignmentCreateUpdateDto updateDto)
        {
            var existingAssignment = await _berthAssignmentRepository.GetByIdAsync(id);
            if (existingAssignment == null)
            {
                throw new KeyNotFoundException($"Berth assignment with ID {id} not found");
            }
            
            // Update fields
            existingAssignment.ContainerId = updateDto.ContainerId;
            existingAssignment.BerthId = updateDto.BerthId;
            if (updateDto.AssignedAt.HasValue)
                existingAssignment.AssignedAt = updateDto.AssignedAt.Value;
            existingAssignment.ReleasedAt = updateDto.ReleasedAt;
            
            var updatedAssignment = await _berthAssignmentRepository.UpdateAsync(existingAssignment);
            return MapToDto(updatedAssignment);
        }

        /// <summary>
        /// Releases a container from a berth
        /// </summary>
        /// <param name="id">The ID of the assignment to release</param>
        /// <returns>The updated assignment</returns>
        public async Task<BerthAssignmentDto> ReleaseContainerAsync(int id)
        {
            var assignment = await _berthAssignmentRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Berth assignment with ID {id} not found");
            }
            
            if (assignment.ReleasedAt.HasValue)
            {
                throw new InvalidOperationException($"Container is already released from berth");
            }
            
            assignment.ReleasedAt = DateTime.UtcNow;
            var updatedAssignment = await _berthAssignmentRepository.UpdateAsync(assignment);
            return MapToDto(updatedAssignment);
        }

        /// <summary>
        /// Deletes a berth assignment
        /// </summary>
        /// <param name="id">The ID of the berth assignment to delete</param>
        /// <returns>True if deletion was successful</returns>
        public async Task<bool> DeleteAsync(object id)
        {
            return await _berthAssignmentRepository.DeleteAsync(id);
        }
        
        /// <summary>
        /// Maps a BerthAssignment entity to a BerthAssignmentDto
        /// </summary>
        /// <param name="assignment">The berth assignment entity</param>
        /// <returns>A berth assignment DTO</returns>
        private static BerthAssignmentDto MapToDto(BerthAssignment assignment)
        {
            return new BerthAssignmentDto
            {
                Id = assignment.Id,
                ContainerId = assignment.ContainerId,
                ContainerName = assignment.Container?.CargoType,
                BerthId = assignment.BerthId,
                BerthName = assignment.Berth?.Name,
                AssignedAt = assignment.AssignedAt,
                ReleasedAt = assignment.ReleasedAt
            };
        }
    }
}