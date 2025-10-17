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
            return await CreateAsync(createDto, 0); // Call the overload with userId = 0 (fallback)
        }

        /// <summary>
        /// Creates a new berth assignment with user ID
        /// </summary>
        /// <param name="createDto">Berth assignment data</param>
        /// <param name="userId">The ID of the user creating the assignment</param>
        /// <returns>The created berth assignment</returns>
        public async Task<BerthAssignmentDto> CreateAsync(BerthAssignmentCreateUpdateDto createDto, int userId)
        {
            // Convert priority number to string
            string priorityString = null;
            if (createDto.Priority.HasValue)
            {
                priorityString = createDto.Priority.Value switch
                {
                    1 => "High",
                    2 => "Medium",
                    3 => "Low",
                    _ => "Medium" // Default to Medium for any other value
                };
            }

            var assignment = new BerthAssignment
            {
                BerthId = createDto.BerthId,
                ShipId = createDto.ShipId,
                ContainerId = createDto.ContainerId,
                AssignmentType = createDto.AssignmentType,
                Priority = priorityString,
                Status = createDto.Status ?? "Scheduled",
                ScheduledArrival = createDto.ScheduledArrival.HasValue 
                    ? DateTime.SpecifyKind(createDto.ScheduledArrival.Value, DateTimeKind.Utc) 
                    : (DateTime?)null,
                ScheduledDeparture = createDto.ScheduledDeparture.HasValue 
                    ? DateTime.SpecifyKind(createDto.ScheduledDeparture.Value, DateTimeKind.Utc) 
                    : (DateTime?)null,
                ContainerCount = createDto.ContainerCount,
                Notes = createDto.Notes ?? string.Empty, // Database requires non-null value
                AssignedAt = DateTime.UtcNow,
                CreatedByUserId = userId > 0 ? userId : (int?)null, // Set user ID if valid
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow // Set to same as CreatedAt on creation
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
            
            // Convert priority number to string
            string priorityString = null;
            if (updateDto.Priority.HasValue)
            {
                priorityString = updateDto.Priority.Value switch
                {
                    1 => "High",
                    2 => "Medium",
                    3 => "Low",
                    _ => "Medium" // Default to Medium for any other value
                };
            }

            // Update fields
            existingAssignment.BerthId = updateDto.BerthId;
            existingAssignment.ShipId = updateDto.ShipId;
            existingAssignment.ContainerId = updateDto.ContainerId;
            existingAssignment.AssignmentType = updateDto.AssignmentType;
            existingAssignment.Priority = priorityString;
            if (!string.IsNullOrEmpty(updateDto.Status))
                existingAssignment.Status = updateDto.Status;
            existingAssignment.ScheduledArrival = updateDto.ScheduledArrival.HasValue 
                ? DateTime.SpecifyKind(updateDto.ScheduledArrival.Value, DateTimeKind.Utc) 
                : (DateTime?)null;
            existingAssignment.ScheduledDeparture = updateDto.ScheduledDeparture.HasValue 
                ? DateTime.SpecifyKind(updateDto.ScheduledDeparture.Value, DateTimeKind.Utc) 
                : (DateTime?)null;
            existingAssignment.ContainerCount = updateDto.ContainerCount;
            existingAssignment.Notes = updateDto.Notes ?? string.Empty; // Database requires non-null value
            existingAssignment.UpdatedAt = DateTime.UtcNow;
            
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