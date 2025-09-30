using Backend.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.Repositories
{
    /// <summary>
    /// Repository interface for BerthAssignment operations
    /// </summary>
    public interface IBerthAssignmentRepository : IRepository<BerthAssignment>
    {
        /// <summary>
        /// Gets assignments by container ID
        /// </summary>
        /// <param name="containerId">The container ID</param>
        /// <returns>Assignments for the specified container</returns>
        Task<IEnumerable<BerthAssignment>> GetByContainerIdAsync(string containerId);
        
        /// <summary>
        /// Gets assignments by berth ID
        /// </summary>
        /// <param name="berthId">The berth ID</param>
        /// <returns>Assignments for the specified berth</returns>
        Task<IEnumerable<BerthAssignment>> GetByBerthIdAsync(int berthId);
        
        /// <summary>
        /// Gets active assignments (where ReleasedAt is null)
        /// </summary>
        /// <returns>All active assignments</returns>
        Task<IEnumerable<BerthAssignment>> GetActiveAssignmentsAsync();
        
        /// <summary>
        /// Gets assignments within a date range
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>Assignments within the specified date range</returns>
        Task<IEnumerable<BerthAssignment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}