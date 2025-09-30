using Backend.DTOs;
using Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    /// <summary>
    /// Service interface for BerthAssignment operations
    /// </summary>
    public interface IBerthAssignmentService : IService<BerthAssignment, BerthAssignmentDto, BerthAssignmentCreateUpdateDto>
    {
        /// <summary>
        /// Gets assignments by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Assignments for the specified container</returns>
        Task<IEnumerable<BerthAssignmentDto>> GetByContainerIdAsync(string containerId);
        
        /// <summary>
        /// Gets assignments by berth ID
        /// </summary>
        /// <param name="berthId">The berth ID</param>
        /// <returns>Assignments for the specified berth</returns>
        Task<IEnumerable<BerthAssignmentDto>> GetByBerthIdAsync(int berthId);
        
        /// <summary>
        /// Gets active assignments (where ReleasedAt is null)
        /// </summary>
        /// <returns>All active assignments</returns>
        Task<IEnumerable<BerthAssignmentDto>> GetActiveAssignmentsAsync();
        
        /// <summary>
        /// Gets assignments within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Assignments within the specified date range</returns>
        Task<IEnumerable<BerthAssignmentDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Releases a container from a berth
        /// </summary>
        /// <param name="id">The ID of the assignment to release</param>
        /// <returns>The updated assignment</returns>
        Task<BerthAssignmentDto> ReleaseContainerAsync(int id);
    }
}